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
[ -z "$COSMOS_DB_KEY" ]      && die "You need to set the COSMOS_DB_KEY environment variable"

AKS_SHORTNAME=$(echo $AKSCLUSTERNAME | sed 's/nhsapp-//')

# Image Tag Overrides
COMMIT_ID="$(git rev-parse HEAD)"
COSMOS_DB_NAME=${COSMOS_DB_NAME:-$TARGET_ENVIRONMENT}
API_IMAGE_TAG=${API_IMAGE_TAG:-$COMMIT_ID}
CID_IMAGE_TAG=${CID_IMAGE_TAG:-$COMMIT_ID}
WEB_IMAGE_TAG=${WEB_IMAGE_TAG:-$COMMIT_ID}
CDSSWIREMOCK_IMAGE_TAG=${CDSSWIREMOCK_IMAGE_TAG:-develop}
SJR_IMAGE_TAG=${SJR_IMAGE_TAG:-$COMMIT_ID}
SJRCONFIG_IMAGE_TAG=${SJRCONFIG_IMAGE_TAG:-$COMMIT_ID}
LOGGER_IMAGE_TAG=${LOGGER_IMAGE_TAG:-$COMMIT_ID}
STUBBED=${STUBBED:-false}
WIREMOCK_IMAGE_TAG=${WIREMOCK_IMAGE_TAG:-2.17.0-alpine}
PERF_WIREMOCK_IMAGE_TAG=${PERF_WIREMOCK_IMAGE_TAG:-latest}
STUB_LOADER_IMAGE_TAG=${STUB_LOADER_IMAGE_TAG:-latest}
USERS_IMAGE_TAG=${USERS_IMAGE_TAG:-$COMMIT_ID}
MESSAGES_IMAGE_TAG=${MESSAGES_IMAGE_TAG:-$COMMIT_ID}
INFO_IMAGE_TAG=${INFO_IMAGE_TAG:-$COMMIT_ID}
RELEASE_CLEANUP=${RELEASE_CLEANUP:-false}

PUBLIC_IP_RESOURCE_GROUP="${AKSCLUSTERNAME}_PIP"
PUBLIC_IP_ADDRESS="$(az network public-ip show --name $PUBLIC_IP_RESOURCE_GROUP --resource-group $PUBLIC_IP_RESOURCE_GROUP | jq '.ipAddress' | tr -d '"')/32"

set -e

# Default to nhsapp app but allow override
CHART=${1:-nhsapp}
CURRENT_RELEASE=$(helm ls --namespace $TARGET_ENVIRONMENT | grep $CHART | awk '{print $1}')

cd nhsapp

info "Target App Namespace: ${TARGET_ENVIRONMENT}"

# 1. Create Namespace if it doesn't exist
[[ `kubectl get namespace | grep -m1 -w "$TARGET_ENVIRONMENT" | awk '{print $2}'` != 'Active' ]] && kubectl create namespace $TARGET_ENVIRONMENT
# Force label consistancy
kubectl label namespace $TARGET_ENVIRONMENT zone=app --overwrite			# Used by LE anisble job
kubectl label namespace $TARGET_ENVIRONMENT letsencrypt=apply --overwrite

# 2. Copy public key to Kubernetes
info "Adding Public Key"
INTERNAL_PUBLIC_KEY=$(kubectl --namespace=cert-manager get secret internal-ca-key-pair -o jsonpath --template '{.data.tls\.crt}' | base64 -d)
if [[ -z ${INTERNAL_PUBLIC_KEY+x} ]]; then
  die "Unable to retrieve internal CA public key from cert-manager namespace"
fi

kubectl get secret -n $TARGET_ENVIRONMENT internal-ca-public-key > /dev/null || kubectl create secret generic internal-ca-public-key --namespace $TARGET_ENVIRONMENT --from-literal=tls.crt="$INTERNAL_PUBLIC_KEY"

# Determine which environment variable to pass as appImageTag--used for ingress rule for web browsers
# to get javascript files during release window. If RELEASE_APP_REPO_TAG exists, use this as this means we
# are deploying a tagged release candidate version; otherwise, this is a MR and the APP_IMAGE_TAG env var
# should be used
if [ -z ${RELEASE_APP_REPO_TAG+x} ]; then
  appImageTag=$APP_IMAGE_TAG
else
  appImageTag=$RELEASE_APP_REPO_TAG
fi

#Set chart and app version
sed -i -e "s|1.0.0|$RELEASE_CHART_REPO_TAG|g" Chart.yaml
sed -i -e "s|APPVERSION|$appImageTag|g" Chart.yaml

# Removing old kubernetes jobs
info "Removing old jobs"
for job in $(kubectl get jobs -n "$TARGET_ENVIRONMENT" -o custom-columns=:.metadata.name)
do
    kubectl delete jobs -n "$TARGET_ENVIRONMENT" "$job"
done


CURRENT_RELEASE=$(helm ls --namespace "$TARGET_ENVIRONMENT" --deployed | grep "$TARGET_ENVIRONMENT" | grep "$CHART" | awk '{print $2}')
if [ "$CURRENT_RELEASE" == "" ]; then
	info "Current release: Not found, assuming initial deployment"
else
	info "Current release: v$CURRENT_RELEASE"
fi

# Deploy Chart install
info "Checking for environment configuration files"
[ -e "../vars/zone/$TARGET_ZONE/vars-$TARGET_ZONE.yaml" ] && {
	info "Zone override file found for $TARGET_ZONE, adding config";
	CUSTOM_VALUES="-f ../vars/zone/$TARGET_ZONE/vars-$TARGET_ZONE.yaml";
}
[ -e "../vars/cluster/$AKS_SHORTNAME.yaml" ] && {
	info "Cluster override found for $AKS_SHORTNAME, adding config";
	CUSTOM_VALUES=$CUSTOM_VALUES" -f ../vars/cluster/vars-$AKS_SHORTNAME.yaml";
}
[ -e "../vars/zone/$TARGET_ZONE/namespace/vars-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml" ] && {
	info "Namespace override found for $TARGET_ZONE/$TARGET_ENVIRONMENT, adding config";
	CUSTOM_VALUES=$CUSTOM_VALUES" -f ../vars/zone/$TARGET_ZONE/namespace/vars-$TARGET_ZONE-$TARGET_ENVIRONMENT.yaml";
}

info "Adding values file params: ${CUSTOM_VALUES}"

# Override stubbed environments if specified
STUBBED_OVERRIDES=""
[ -n "$STUBBED_CID" ] && {
	info "Overriding cidStubbed to be $STUBBED_CID"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.cidStubbed=$STUBBED_CID"
}
[ -n "$STUBBED_EMIS" ] && {
	info "Overriding emisStubbed to be $STUBBED_EMIS"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.emisStubbed=$STUBBED_EMIS"
}
[ -n "$STUBBED_VISION" ] && {
	info "Overriding visionStubbed to be $STUBBED_VISION"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.visionStubbed=$STUBBED_VISION"
}
[ -n "$STUBBED_ORGANDONATION" ] && {
	info "Overriding organDonationStubbed to be $STUBBED_ORGANDONATION"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.organDonationStubbed=$STUBBED_ORGANDONATION"
}
[ -n "$STUBBED_BROTHERMAILER" ] && {
	info "Overriding organDonationStubbed to be $STUBBED_BROTHERMAILER"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.brothermailerStubbed=$STUBBED_BROTHERMAILER"
}
[ -n "$STUBBED_CDSS" ] && {
	info "Overriding cdssStubbed to be $STUBBED_CDSS"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.cdssStubbed=$STUBBED_CDSS"
}
[ -n "$STUBBED_WIREMOCK" ] && {
	info "Overriding deployWiremock to be $STUBBED_WIREMOCK"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.deployWiremock=$STUBBED_WIREMOCK"
}
[ -n "$STUBBED_TPP" ] && {
	info "Overriding tppStubbed to be $STUBBED_TPP"
	STUBBED_OVERRIDES=$STUBBED_OVERRIDES" --set environmentStubbed.tppStubbed=$STUBBED_TPP"
}
info "Adding stubbed overrides: ${STUBBED_OVERRIDES}"

# Delete existing Pod Disruption Budgets before invoking chart as PDB values cannot be updated
PDB_NAMES=$(kubectl get pdb -n "$TARGET_ENVIRONMENT" -o custom-columns=:.metadata.name)
if [[ -n $PDB_NAMES ]]; then
	for pdb in $PDB_NAMES
	do
	 	if [[ "$pdb" == *"sjr"* ]]; then
		 	# skip deletion of PDB for SJR as that now has its own chart
			continue
		fi

		kubectl delete pdb "$pdb" -n "$TARGET_ENVIRONMENT"
	done
fi

if [[ "$RELEASE_CLEANUP" == "true" ]] && [[ "$TARGET_ENVIRONMENT" != "production" ]] && [ "$TARGET_ENVIRONMENT" != "staging" ] && [[ $(helm ls --all --all-namespaces | grep $TARGET_ENVIRONMENT-) ]]; then
	info "Deleting releases in $TARGET_ENVIRONMENT-"
	helm ls --short --all --all-namespaces | grep "$TARGET_ENVIRONMENT"- | xargs helm delete --namespace $TARGET_ENVIRONMENT && sleep 10
fi

info "Checking if TLS Secret certificate exists in namespace"
if ! kubectl get secrets -n "$TARGET_ENVIRONMENT" | grep -qm1 'nhsapp-service-nhs-uk'; then
	info "Copying TLS Secret from App Publishing Namespace"
    TLS_CERT_SECRET_NAME=$(kubectl get secrets -n app-publishing | grep -m1 'nhsapp-service-nhs-uk' | cut -d ' ' -f 1)
    [ "${TLS_CERT_SECRET_NAME}" != "" ] && kubectl get secret "$TLS_CERT_SECRET_NAME" --namespace=app-publishing --export -o yaml | kubectl apply --namespace="$TARGET_ENVIRONMENT" -f -
fi

info "Beginning deployment -- $TARGET_ENVIRONMENT-$appImageTag..."
helm upgrade "$TARGET_ENVIRONMENT-$appImageTag" \
	--set global.environment="$TARGET_ENVIRONMENT" \
	--set appImageTag="$appImageTag" \
	--set cid.image_tag="$CID_IMAGE_TAG" \
	--set api.image_tag="$API_IMAGE_TAG" \
	--set web.image_tag="$WEB_IMAGE_TAG" \
	--set cdsswiremock.image_tag="$CDSSWIREMOCK_IMAGE_TAG" \
	--set logger.image_tag="$LOGGER_IMAGE_TAG" \
	--set sjr.image_tag="$SJR_IMAGE_TAG" \
	--set sjrconfig.image_tag="$SJRCONFIG_IMAGE_TAG" \
	--set stub_loader.image_tag="$STUB_LOADER_IMAGE_TAG" \
	--set wiremock.image_tag="$PERF_WIREMOCK_IMAGE_TAG" \
	--set users.image_tag=$USERS_IMAGE_TAG \
	--set messages.image_tag=$MESSAGES_IMAGE_TAG \
	--set info.image_tag=$INFO_IMAGE_TAG \
	--set zone="$TARGET_ZONE" \
	--set clusterIP="$PUBLIC_IP_ADDRESS" \
	--set cosmosDBName="$COSMOS_DB_NAME" \
	--set region="$REGION" \
	$STUBBED_OVERRIDES \
	--namespace $TARGET_ENVIRONMENT \
	--reset-values \
	--install \
	$CUSTOM_VALUES \
	.

DEPLOYMENT_SUCCESS=$?

[ "$DEPLOYMENT_SUCCESS" -ne "0" ] && [ "$CURRENT_RELEASE" != "" ] && {
	info "Failed release detected, performing rollback to v$CURRENT_RELEASE"
	helm rollback "$TARGET_ENVIRONMENT-$appImageTag" --namespace $TARGET_ENVIRONMENT --force "$CURRENT_RELEASE";
}

info "Deployment Completed"