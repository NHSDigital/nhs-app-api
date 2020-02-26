#!/usr/bin/env bash
# Envvars:
# - $AKSCLUSTERNAME - The cluster you are deploying to
# - $TARGET_ENVIRONMENT - The environment you are creating (namespace)

source script_helpers/dependency_check
source script_helpers/formatting
source script_helpers/aks_access
source script_helpers/unlock_repo

unlock_repo

[ -z "$AKSCLUSTERNAME" ]     && die "You need to set the AKSCLUSTERNAME environment variable"
[ -z "$TARGET_ENVIRONMENT" ] && die "You need to set the TARGET_ENVIRONMENT environment variable"
[ -z "$TARGET_ZONE" ]        && die "You need to set the TARGET_ZONE environment variable"

# Read what versions are currently live
LIVE_WEB_IMAGE_TAG=$(kubectl get svc -n "$TARGET_ZONE" default-www -o jsonpath="{.spec.selector.version}" | awk '{print $1}')
LIVE_API_IMAGE_TAG=$(kubectl get svc -n "$TARGET_ZONE" default-api -o jsonpath="{.spec.selector.version}" | awk '{print $1}')
LIVE_CID_IMAGE_TAG=$(kubectl get svc -n "$TARGET_ZONE" default-cid -o jsonpath="{.spec.selector.version}" | awk '{print $1}')
LIVE_SJR_IMAGE_TAG=$(kubectl get svc -n "$TARGET_ZONE" nhsonline-sjr -o jsonpath="{.spec.selector.version}" | awk '{print $1}')

if [ -z ${RELEASE_APP_REPO_TAG+x} ]; then
  appImageTag=$APP_IMAGE_TAG
else
  appImageTag=$RELEASE_APP_REPO_TAG
fi

set -e

# Set up app date vars
RETENTIONDATE=$(date --date="2 days ago" +%y%m%d)
RELEASEDATERAW=$(kubectl get deployment -n "$TARGET_ZONE" nhsonline-www-"${LIVE_WEB_IMAGE_TAG//./-}" -o jsonpath='{.metadata.creationTimestamp}')
RELEASEDATE=$(date --date="$RELEASEDATERAW" +%y%m%d)

# If the live release is newer than 2 days then don not remove old releases
if [ $RELEASEDATE -ge $RETENTIONDATE ] ; then 
	info "The live app release has been deployed less than 2 days. No releases to be deleted"
else
	#Get all deployed releases in app namespace except live release
	RELEASES=$(helm ls -a --namespace "$TARGET_ZONE" | awk 'NR > 1 {print $1}' | grep -v "$TARGET_ZONE-$appImageTag" | grep -v "$TARGET_ZONE-sjr" || true )

	if [ -z "$RELEASES" ]; then
		info "No app releases to be deleted"
	else
		#Check if version to be deleted is currently live. If not live, delete it.
		for release in $RELEASES
		do
			VERSION_TO_DELETE=${release//"$TARGET_ZONE"-/}
			[[ "$LIVE_WEB_IMAGE_TAG" == "$VERSION_TO_DELETE" ]] && info "$VERSION_TO_DELETE of web is currently live and cannot be removed." && STATUS=Failed
			[[ "$LIVE_API_IMAGE_TAG" == "$VERSION_TO_DELETE" ]] && info "$VERSION_TO_DELETE of api is currently live and cannot be removed." && STATUS=Failed
			[[ "$LIVE_CID_IMAGE_TAG" == "$VERSION_TO_DELETE" ]] && info "$VERSION_TO_DELETE of cid is currently live and cannot be removed." && STATUS=Failed
   			[[ $STATUS == 'Failed' ]] && die "Some of the resources being removed are currently live and cannot be removed. Check the above build log."
    		helm del "$release" --namespace "$TARGET_ZONE"
		done
	fi
fi

if [ -z ${RELEASE_SJR_RELEASE_TAG+x} ]; then
  sjrImageTag=$APP_IMAGE_TAG
else
  sjrImageTag=$RELEASE_SJR_RELEASE_TAG
fi

# Set up sjr date vars
SJRRELEASEDATERAW=$(kubectl get deployment -n "$TARGET_ZONE" nhsonline-sjr-"${LIVE_SJR_IMAGE_TAG//./-}" -o jsonpath='{.metadata.creationTimestamp}')
SJRRELEASEDATE=$(date --date="$SJRRELEASEDATERAW" +%y%m%d)

# If the live release is newer than 2 days then don not remove old releases
if [ $SJRRELEASEDATE -ge $RETENTIONDATE ] ; then 
	info "The live SJR release has been deployed less than 2 days. No releases to be deleted"
else
	#Get all deployed sjr releases in app namespace except live & default release
	SJRRELEASES=$(helm ls -a --namespace "$TARGET_ZONE" | awk 'NR > 1 {print $1}' | grep "sjr" | grep -v "$TARGET_ZONE-sjr-$sjrImageTag" | grep -v "$TARGET_ZONE-sjr-service" || true )

	if [ -z "$SJRRELEASES" ]; then
		info "No sjr releases to be deleted"
	else
		#Check if version to be deleted is currently live. If not live, delete it.
		for sjrrelease in $SJRRELEASES
		do
			VERSION_TO_DELETE=${sjrrelease//"$TARGET_ZONE"-sjr/}
			[[ "$LIVE_SJR_IMAGE_TAG" == "$VERSION_TO_DELETE" ]] && info "$VERSION_TO_DELETE of sjr is currently live and cannot be removed." && STATUS=Failed
   			[[ $STATUS == 'Failed' ]] && die "Some of the resources being removed are currently live and cannot be removed. Check the above build log."
    		helm del "$sjrrelease" --namespace "$TARGET_ZONE"
		done
	fi
fi

info "Clean Up Completed"