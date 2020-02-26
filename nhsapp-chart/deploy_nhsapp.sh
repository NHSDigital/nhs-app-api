#!/usr/bin/env bash
# Envvars:
# - $AKSCLUSTERNAME - The cluster you are deploying to
# - $TARGET_ENVIRONMENT - The environment you are creating (namespace)
source nhsapp-chart/script_helpers/dependency_check
source nhsapp-chart/script_helpers/formatting
source nhsapp-chart/script_helpers/aks_access
source nhsapp-chart/script_helpers/unlock_repo

function trigger_sjr() {
    info "Triggering SJR deployment job for $TARGET_ENVIRONMENT"

	case $TARGET_ENVIRONMENT in
	"ops"*)
		BUILD_CONFIG_ID="nhsapp_Deployment_2NonLiveSubscription_1Sandbox_Namespaces_1OpsSelfService_DeploySjr"
	;;
	"scratch"*)
		BUILD_CONFIG_ID="NHSOnline_Deployment_2NonLiveSubscription_Development_TeamDevelopmentEnvironment_2DeploySjr"
	;;
	"preview")
		BUILD_CONFIG_ID="NHSOnline_Deployment_2NonLiveSubscription_Development_Namespaces_1Preview_2_2DeploySjr"
	;;
	"demo")
		if [ $TARGET_ZONE = "dev" ]; then
			BUILD_CONFIG_ID="NHSOnline_Deployment_2NonLiveSubscription_Development_Namespaces_3Demo_2DeploySjr"
		elif [ $TARGET_ZONE = "staging" ]; then
			BUILD_CONFIG_ID="NHSOnline_Deployment_3LiveSubscription_2Staging_Namespaces_4Demo_2DeploySjr"
		else
			die "Unknown target zone $TARGET_ZONE."
		fi
	;;
	"stubbed"*)
		BUILD_CONFIG_ID="NHSOnline_Deployment_3LiveSubscription_2Staging_Namespaces_3Stubbed_2DeploySjr"
	;;
	"staging")
		BUILD_CONFIG_ID="NHSOnline_Deployment_3LiveSubscription_2Staging_Namespaces_6Staging_2DeploySjr"
	;;
	"production")
		BUILD_CONFIG_ID="NHSOnline_Deployment_3LiveSubscription_3Production_Namespaces_Production_2DeploySjr"
	;;
	*)
		die "Unable to find Build Config ID for $TARGET_ENVIRONMENT"
	;;
	esac

	if [[ $TARGET_ZONE == "dev" || $TARGET_ENVIRONMENT == "stubbed"* ]]; then
		# Prepare JSON data containing required parameters - including passing through the app version
		DATA=$(cat <<-EOF
		{"buildType":{"id":"${BUILD_CONFIG_ID}","projectId":"NHSOnline"},"triggeringOptions":{"queueAtTop":true},"properties":{"property":[{"name":"env.TARGET_ENVIRONMENT","value":"${TARGET_ENVIRONMENT}"},{"name":"env.SJR_IMAGE_TAG","value":"${APP_IMAGE_TAG}"},{"name":"env.SJRCONFIG_IMAGE_TAG","value":"${APP_IMAGE_TAG}"},{"name":"env.REGION","value":"${REGION}"},{"name":"env.AKSCLUSTERNAME","value":"${AKSCLUSTERNAME}"}]}}
		EOF
		)
	else
		# Prepare JSON data containing required parameters
		DATA=$(cat <<-EOF
		{"buildType":{"id":"${BUILD_CONFIG_ID}","projectId":"NHSOnline"},"triggeringOptions":{"queueAtTop":true},"properties":{"property":[{"name":"env.TARGET_ENVIRONMENT","value":"${TARGET_ENVIRONMENT}"},{"name":"env.REGION","value":"${REGION}"},{"name":"env.AKSCLUSTERNAME","value":"${AKSCLUSTERNAME}"}]}}
		EOF
		)
	fi

	# Curl TeamCity buildQueue endpoint to trigger job with data
	curl -s -u system:$TC_SYSTEM_PW --request POST "http://teamcity.teamcity.svc.cluster.local:8111/httpAuth/app/rest/buildQueue" --header "Content-Type: application/json" -H "Accept: application/json" -d $DATA
}


[ -z "$AKSCLUSTERNAME" ]     && die "You need to set the AKSCLUSTERNAME environment variable"
[ -z "$TARGET_ENVIRONMENT" ] && die "You need to set the TARGET_ENVIRONMENT environment variable"
[ -z "$TARGET_ZONE" ]        && die "You need to set the TARGET_ZONE environment variable"
[ -z "$COSMOS_DB_KEY" ]      && die "You need to set the COSMOS_DB_KEY environment variable"

AKS_SHORTNAME=$(echo $AKSCLUSTERNAME | sed 's/nhsapp-//')

# Image Tag Overrides
COSMOS_DB_NAME=${COSMOS_DB_NAME:-$TARGET_ENVIRONMENT}
API_IMAGE_TAG=${API_IMAGE_TAG:-latest}
CID_IMAGE_TAG=${CID_IMAGE_TAG:-latest}
WEB_IMAGE_TAG=${WEB_IMAGE_TAG:-latest}
CDSSWIREMOCK_IMAGE_TAG=${CDSSWIREMOCK_IMAGE_TAG:-latest}
SJR_IMAGE_TAG=${SJR_IMAGE_TAG:-latest}
SJRCONFIG_IMAGE_TAG=${SJRCONFIG_IMAGE_TAG:-latest}
LOGGER_IMAGE_TAG=${LOGGER_IMAGE_TAG:-latest}
STUBBED=${STUBBED:-false}
WIREMOCK_IMAGE_TAG=${WIREMOCK_IMAGE_TAG:-2.17.0-alpine}
PERF_WIREMOCK_IMAGE_TAG=${PERF_WIREMOCK_IMAGE_TAG:-latest}
STUB_LOADER_IMAGE_TAG=${STUB_LOADER_IMAGE_TAG:-latest}
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
[[ `kubectl get namespace | grep -m1 $TARGET_ENVIRONMENT | awk '{print $2}'` != 'Active' ]] && kubectl create namespace $TARGET_ENVIRONMENT
# Force label consistancy
kubectl label namespace $TARGET_ENVIRONMENT zone=app --overwrite			# Used by LE anisble job
kubectl label namespace $TARGET_ENVIRONMENT letsencrypt=apply --overwrite

# 2. Copy public key to Kubernetes
info "Adding Public Key"
INTERNAL_PUBLIC_KEY=$(kubectl --namespace=kube-system get secret internal-ca-key-pair -o jsonpath --template '{.data.tls\.crt}' | base64 -d)
if [ -z ${INTERNAL_PUBLIC_KEY+x} ]; then
  die "Unable to retrieve internal CA public key from kube-system namespace"
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

# For Sandbox and Dev zones, we will use shared Request Units (RU/s) across all collections
# For Staging and Production, we will use provisioned RU/s for each collection
if [[ $TARGET_ZONE == "sandbox" ]] || [[ $TARGET_ZONE == "dev" ]]; then
		# vars for when we migrate to shared throughput model
    # [[ $TARGET_ZONE == "sandbox" ]] && THROUGHPUT=400
    # [[ $TARGET_ZONE == "dev" ]] && THROUGHPUT=4000
	info "CosmosDB - Creating SQL API Database $COSMOS_DB_NAME"
	[[ $(az cosmosdb sql database list --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "$COSMOS_DB_NAME") ]] || az cosmosdb sql database create --name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" #--throughput $THROUGHPUT

	info "CosmosDB - Creating Audit container"
	[[ $(az cosmosdb sql container list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "audit") ]] || az cosmosdb sql container create --name audit --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --partition-key-path "/NhsNumber" --throughput 400

	info "CosmosDB - Create Terms & Conditions container"
	[[ $(az cosmosdb sql container list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "consent") ]] || az cosmosdb sql container create --name consent --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --partition-key-path "/NhsNumber" --throughput 400

	info "CosmosDB - Creating Mongo API Database $COSMOS_DB_NAME"
	[[ $(az cosmosdb mongodb database list --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "$COSMOS_DB_NAME") ]] || az cosmosdb mongodb database create --name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}"

	info "CosmosDB - Creating Session Collection"
	[[ $(az cosmosdb mongodb collection list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "session") ]] || az cosmosdb mongodb collection create --name session --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --shard "_id" --throughput 400

	info "CosmosDB - Creating im1cache Collection"
	[[ $(az cosmosdb mongodb collection list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "im1cache") ]] || az cosmosdb mongodb collection create --name im1cache --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --shard "_id" --throughput 400

elif [[ $TARGET_ZONE == "staging" ]] || [[ $TARGET_ZONE == "production" ]]; then
	info "CosmosDB - Creating SQL API Database $COSMOS_DB_NAME"
	[[ $(az cosmosdb sql database list --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "$COSMOS_DB_NAME") ]] || az cosmosdb sql database create --name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}"

	info "CosmosDB - Creating Audit container"
	[[ $(az cosmosdb sql container list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "audit") ]] || az cosmosdb sql container create --name audit --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --throughput 800 --partition-key-path "/NhsNumber"

	info "CosmosDB - Create Terms & Conditions container"
	[[ $(az cosmosdb sql container list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "consent") ]] || az cosmosdb sql container create --name consent --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}" --resource-group "nhsapp-${TARGET_ZONE}" --throughput 400 --partition-key-path "/NhsNumber"

	info "CosmosDB - Creating Mongo API Database $COSMOS_DB_NAME"
	[[ $(az cosmosdb mongodb database list --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "$COSMOS_DB_NAME") ]] || az cosmosdb mongodb database create --name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}"

	info "CosmosDB - Creating Session Collection"
	[[ $(az cosmosdb mongodb collection list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "session") ]] || az cosmosdb mongodb collection create --name session --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --throughput 10000 --default-ttl 600

	info "CosmosDB - Creating im1cache Collection"
	[[ $(az cosmosdb mongodb collection list --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --query '[].name' -o tsv | grep -w "im1cache") ]] || az cosmosdb mongodb collection create --name im1cache --database-name "$COSMOS_DB_NAME" --account-name "nhsapp-${TARGET_ZONE}-mongo" --resource-group "nhsapp-${TARGET_ZONE}" --throughput 400 --default-ttl 604800
fi

CURRENT_RELEASE=$(helm ls --namespace "$TARGET_ENVIRONMENT" --deployed | grep "$TARGET_ENVIRONMENT" | grep "$CHART" | awk '{print $2}')
if [ "$CURRENT_RELEASE" == "" ]; then
	info "Current release: Not found, assuming initial deployment"
else
	info "Current release: v$CURRENT_RELEASE"
fi

# Deploy Chart install
# Environment secrets, e.g. dev, staging, production
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

info "Beginning deployment -- $TARGET_ENVIRONMENT-$appImageTag..."
helm upgrade "$TARGET_ENVIRONMENT-$appImageTag" \
	-f "nhsapp-ops-vault/secrets/zone/$TARGET_ZONE/secrets-$TARGET_ZONE.yaml" \
	--set global.environment="$TARGET_ENVIRONMENT" \
	--set appImageTag="$appImageTag" \
	--set cid.image_tag="$CID_IMAGE_TAG" \
	--set api.image_tag="$API_IMAGE_TAG" \
	--set web.image_tag="$WEB_IMAGE_TAG" \
	--set cdsswiremock.image_tag="$CDSSWIREMOCK_IMAGE_TAG" \
	--set logger.image_tag="$LOGGER_IMAGE_TAG" \
	--set stub_loader.image_tag="$STUB_LOADER_IMAGE_TAG" \
	--set wiremock.image_tag="$PERF_WIREMOCK_IMAGE_TAG" \
	--set zone="$TARGET_ZONE" \
	--set clusterIP="$PUBLIC_IP_ADDRESS" \
	--set cosmosDBName="$COSMOS_DB_NAME" \
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

if [[ "$TARGET_ZONE" == production || "$TARGET_ENVIRONMENT" == "staging" ]]; then
	[[ "$TARGET_ZONE" == production ]] && info "Target zone is Production, not triggering SJR deployment."
	[[ "$TARGET_ENVIRONMENT" == "staging" ]] && info "Target environment is Staging, not triggering SJR deployment."
else
	info "Deploying SJR into namespace $TARGET_ENVIRONMENT"
  trigger_sjr
fi
