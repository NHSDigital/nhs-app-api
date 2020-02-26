#!/usr/bin/env bash
# Envvars:
# - $AKSCLUSTERNAME - The cluster you are deploying to
# - $TARGET_ENVIRONMENT - The environment you are creating (namespace)
source script_helpers/dependency_check
source script_helpers/formatting
source script_helpers/aks_access
source script_helpers/unlock_repo

[ -z "$AKSCLUSTERNAME" ]     && die "You need to set the AKSCLUSTERNAME environment variable"
[ -z "$TARGET_ENVIRONMENT" ] && die "You need to set the TARGET_ENVIRONMENT environment variable"
[ -z "$TARGET_ZONE" ]        && die "You need to set the TARGET_ZONE environment variable"

AKS_SHORTNAME=$(echo $AKSCLUSTERNAME | sed 's/nhsapp-//')

# Image Tag Overrides
SJR_IMAGE_TAG=${SJR_IMAGE_TAG:-latest}
SJRCONFIG_IMAGE_TAG=${SJRCONFIG_IMAGE_TAG:-latest}
RELEASE_CLEANUP=${RELEASE_CLEANUP:-false}


if [ -z ${RELEASE_SJR_RELEASE_TAG+x} ]; then
  appImageTag=$APP_IMAGE_TAG
else
  appImageTag=$RELEASE_SJR_RELEASE_TAG
fi

PUBLIC_IP_RESOURCE_GROUP="${AKSCLUSTERNAME}_PIP"
PUBLIC_IP_ADDRESS="$(az network public-ip show --name $PUBLIC_IP_RESOURCE_GROUP --resource-group $PUBLIC_IP_RESOURCE_GROUP | jq '.ipAddress' | tr -d '"')/32"

set -e

# Default to nhsapp-sjr but allow override
CHART=${1:-nhsapp-sjr}
CURRENT_RELEASE=$(helm ls --namespace $TARGET_ENVIRONMENT | grep $CHART | awk '{print $1}')

pushd nhsapp-sjr

# Clone nhsapp-ops-vault repository
info "Cloning nhsapp-ops-vault repo"
### Azure DevOps repo code read-only token valid till: 7th Feb 2021 ixso7tesye5wk23eku5f2kopaacyin2ex3hjwwjavh4dkat5xcrq
VAULT_REPO_URL="https://oauth2:ixso7tesye5wk23eku5f2kopaacyin2ex3hjwwjavh4dkat5xcrq@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp-ops-vault"
RELEASE_VAULT_REPO_TAG=${RELEASE_VAULT_REPO_TAG:-develop}
git clone --single-branch --branch "$RELEASE_VAULT_REPO_TAG" "$VAULT_REPO_URL"

# Unlock secrets repo
pushd nhsapp-ops-vault
unlock_repo
popd

info "Target App Namespace: ${TARGET_ENVIRONMENT}"

# 1. Create Namespace if it doesn't exist
[[ $(kubectl get namespace | grep -m1 $TARGET_ENVIRONMENT | awk '{print $2}') != 'Active' ]] && kubectl create namespace $TARGET_ENVIRONMENT
# Force label consistancy
kubectl label namespace $TARGET_ENVIRONMENT zone=app --overwrite			# Used by LE anisble job
kubectl label namespace $TARGET_ENVIRONMENT letsencrypt=apply --overwrite

# 2. Copy public key to Kubernetes
info "Adding Public Key"
INTERNAL_PUBLIC_KEY=$(kubectl --namespace=kube-system get secret internal-ca-key-pair -o jsonpath --template '{.data.tls\.crt}' | base64 -d)
kubectl get secret -n $TARGET_ENVIRONMENT internal-ca-public-key > /dev/null || kubectl create secret generic internal-ca-public-key --namespace $TARGET_ENVIRONMENT --from-literal=tls.crt="$INTERNAL_PUBLIC_KEY"

#Set chart and app version
sed -i -e "s|1.0.0|$appImageTag|g" Chart.yaml
sed -i -e "s|APPVERSION|$appImageTag|g" Chart.yaml

# Removing old kubernetes jobs
info "Removing old jobs"
for job in $(kubectl get jobs -n $TARGET_ENVIRONMENT -o custom-columns=:.metadata.name)
do
    kubectl delete jobs -n "$TARGET_ENVIRONMENT" "$job"
done

CURRENT_RELEASE=$(helm ls --namespace "$TARGET_ENVIRONMENT" --deployed | grep "$TARGET_ENVIRONMENT" | grep "$CHART" | awk '{print $2}')
if [ "$CURRENT_RELEASE" == "" ]; then
	info "Current release: Not found, assuming initial deployment"
else
	info "Current release: v$CURRENT_RELEASE"
fi

info "Checking for environment configuration files"
[ -e "nhsapp-ops-vault/vars/zone/$TARGET_ZONE/vars-$TARGET_ZONE.yaml" ] && {
	info "Zone override file found for $TARGET_ZONE, adding config";
	CUSTOM_VALUES="-f nhsapp-ops-vault/vars/zone/$TARGET_ZONE/vars-$TARGET_ZONE.yaml";
}
[ -e "nhsapp-ops-vault/vars/cluster/$AKS_SHORTNAME.yaml" ] && {
	info "Cluster override found for $AKS_SHORTNAME, adding config";
	CUSTOM_VALUES=$CUSTOM_VALUES" -f nhsapp-ops-vault/vars/cluster/vars-$AKS_SHORTNAME.yaml";
}
[ -e "nhsapp-ops-vault/vars/zone/$TARGET_ZONE/namespace/vars-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml" ] && {
	info "Namespace override found for $TARGET_ZONE/$TARGET_ENVIRONMENT, adding config";
	CUSTOM_VALUES=$CUSTOM_VALUES" -f nhsapp-ops-vault/vars/zone/$TARGET_ZONE/namespace/vars-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml";
}


info "Adding values file params: ${CUSTOM_VALUES}"

# Delete existing Pod Disruption Budget for SJR before invoking chart as PDB value cannot be updated
info "Checking for PDBs..."
PDB=$(kubectl get pdb -n "$TARGET_ENVIRONMENT" -o custom-columns=:.metadata.name | grep 'sjr' || true)
if [[ -n $PDB ]]; then
	info "Deleting existing Pod Disruption Budget for SJR"
	kubectl delete pdb $PDB -n "$TARGET_ENVIRONMENT"
fi

if [[ "$RELEASE_CLEANUP" == "true" ]] && [[ "$TARGET_ENVIRONMENT" != "production" ]] && [ "$TARGET_ENVIRONMENT" != "staging" ] && [[ $(helm ls --all --all-namespaces | grep $TARGET_ENVIRONMENT-sjr) ]]; then
	info "Deleting releases in $TARGET_ENVIRONMENT-"
	helm ls --short --all --all-namespaces | grep "${TARGET_ENVIRONMENT}-sjr" | xargs helm delete --namespace $TARGET_ENVIRONMENT && sleep 10
fi

# Deploy Chart install
info "Checking for $TARGET_ENVIRONMENT specific secrets"
if [ -e "nhsapp-ops-vault/secrets/zone/$TARGET_ZONE/namespace/secrets-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml" ]; then
	info "Deploying $CHART-$appImageTag with $TARGET_ENVIRONMENT secrets"
	CUSTOM_VALUES=$CUSTOM_VALUES" -f nhsapp-ops-vault/secrets/zone/$TARGET_ZONE/namespace/secrets-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml"
else
	info "Deploying $CHART-$appImageTag with no $TARGET_ENVIRONMENT override secrets"
fi

info "Checking for $REGION specific secrets"
if [ "$TARGET_ZONE" == "production" ] || [ "$TARGET_ZONE" == "staging" ]; then
	info "Deploying $CHART-$appImageTag with $REGION secrets"
	CUSTOM_VALUES=$CUSTOM_VALUES" -f nhsapp-ops-vault/secrets/zone/$TARGET_ZONE/region/secrets-$TARGET_ZONE-$REGION.yaml"
else
	info "Deploying $CHART-$appImageTag with no $REGION override secrets"
fi

# Check if service has been deployed in this namespace; if not, deploy it.
SVC=$(kubectl get svc -n $TARGET_ENVIRONMENT nhsapp-sjr || true)
if [[ -n $SVC ]]; then
	info "SJR Service has already been deployed. Proceeding to deployment of SJR API."
else
	info "Deploying SJR service"
	info "Beginning deployment..."
	pushd ../nhsapp-sjr-service

	#Set chart and app version
	sed -i -e "s|1.0.0|$appImageTag|g" Chart.yaml
	sed -i -e "s|APPVERSION|$appImageTag|g" Chart.yaml
	helm upgrade "$TARGET_ENVIRONMENT-sjr-service" \
		--set global.environment="$TARGET_ENVIRONMENT" \
		--set zone="$TARGET_ZONE" \
		--namespace "$TARGET_ENVIRONMENT" \
		--reset-values \
		--install \
		.

	DEPLOYMENT_SUCCESS=$?

	[ "$DEPLOYMENT_SUCCESS" -ne "0" ] && [ "$CURRENT_RELEASE" != "" ] && {
		info "Failed release detected, performing rollback to v$CURRENT_RELEASE"
		helm rollback "$TARGET_ENVIRONMENT-sjr-service" --namespace $TARGET_ENVIRONMENT --force "$CURRENT_RELEASE";
	}
	popd
fi

info "Beginning deployment..."
helm upgrade "$TARGET_ENVIRONMENT-sjr-$appImageTag" \
	-f "nhsapp-ops-vault/secrets/zone/$TARGET_ZONE/secrets-$TARGET_ZONE.yaml" \
	--set global.environment="$TARGET_ENVIRONMENT" \
	--set appImageTag="$appImageTag" \
	--set sjr.image_tag="$SJR_IMAGE_TAG" \
	--set sjrconfig.image_tag="$SJRCONFIG_IMAGE_TAG" \
	--set zone="$TARGET_ZONE" \
	--set clusterIP="$PUBLIC_IP_ADDRESS" \
	--namespace "$TARGET_ENVIRONMENT" \
	--reset-values \
	--install \
	$CUSTOM_VALUES \
	--wait \
	--timeout 5m \
	.

DEPLOYMENT_SUCCESS=$?

[ "$DEPLOYMENT_SUCCESS" -ne "0" ] && [ "$CURRENT_RELEASE" != "" ] && {
	info "Failed release detected, performing rollback to v$CURRENT_RELEASE"
	helm rollback "$TARGET_ENVIRONMENT-sjr-$appImageTag" --namespace $TARGET_ENVIRONMENT --force "$CURRENT_RELEASE";
	exit 1
}

info "SJR Deployment Completed"

# Point SJR service to this release automatically
info "Updating Service to point to SJR version $appImageTag"
kubectl patch svc -n $TARGET_ENVIRONMENT nhsapp-sjr -p '{"spec":{"selector":{"version":"'${appImageTag}'"}}}'

info "Updating Service Label"
kubectl label svc -n $TARGET_ENVIRONMENT nhsapp-sjr version=$appImageTag --overwrite
