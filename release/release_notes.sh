#!/bin/bash

set -e

function die () {
  echo >&2 "===]> Error: $@ "
  exit 1
}

function info () {
  echo >&2 "===]> Info: $@ ";
}

function displayUsage () {
  echo "Usage: ./release_notes.sh [-h]

  Create release notes for the version set in the environment variable TAG. TAG must be in the format
  v.[Major].[Minor].[Patch][Ignored Optional Postfix] e.g. v1.8.0 or v1.8.0-RC1
  The release notes will be created in a file in the format <TAG>.txt against the associated previous
  release. i.e if it is a minor update, the release notes will be created against the previous minor version.

  Flags:
    -h     Display this help message
  "
  exit 1
}

while getopts "h" opt; do

  case $opt in
    h)
      displayUsage
      ;;
    \?)
      displayUsage
      ;;
  esac
done

if [ -z $TAG ]
then
  die "TAG is not set"
fi


read VERSION_PARTS[{1..3}] <<<  $(echo "$TAG" | sed 's/v\([0-9.]*\)\(.*\)/\1/' | awk -F \. {'print $1,$2, $3'})

info "v${VERSION_PARTS[1]}.${VERSION_PARTS[2]}.${VERSION_PARTS[3]}";

if [ "${VERSION_PARTS[3]}" -ne "0" ]; then
  info "Patch Update";
  PATCH_BELOW=$((VERSION_PARTS[3] - 1))
  VERSION_BELOW="v${VERSION_PARTS[1]}.${VERSION_PARTS[2]}.${PATCH_BELOW}"
elif [ "${VERSION_PARTS[2]}" -ne "0" ]; then
  info "Minor Update"
  MINOR_BELOW=$((VERSION_PARTS[2] - 1))
  VERSION_BELOW="v${VERSION_PARTS[1]}.${MINOR_BELOW}.${VERSION_PARTS[3]}"
else
  info "Major Update"
  MAJOR_BELOW=$((VERSION_PARTS[1] - 1))
  VERSION_BELOW="v${MAJOR_BELOW}.${VERSION_PARTS[2]}.${VERSION_PARTS[3]}"
fi

info "Comparing $TAG to $VERSION_BELOW"

WEBVER="1.XX.0"
IOSVER="1.XX.0"
ANDROIDVER="1.XX.0"

GITLOG=`git log --pretty=format:"%s" $VERSION_BELOW..$TAG | awk 'ORS="<br/>"' | sed "s/\"/'/g"`

IMPLEMENTATIONDATE=`date -dnext-monday +%d-%m-%Y`

SUBJECT="New RFC : NHS App Release - $TAG"
MESSAGE="Change Notification for NHS APP<br><br>"
MESSAGE="${MESSAGE}This change is approved by Solution Assurance and approval has been received from all necessary areas<br><br><br>"
MESSAGE="${MESSAGE}<b>SERVICE AFFECTED</b> : NHS App<br>"
MESSAGE="${MESSAGE}<b>PRIORITY</b> : Standard Change<br>"
MESSAGE="${MESSAGE}<b>DESCRIPTION</b> : Standard Feature Release for NHS App<br>"
MESSAGE="${MESSAGE}${GITLOG}<br><br>"
MESSAGE="${MESSAGE}<b>PRIMARY CI</b> : NHS App<br>"
MESSAGE="${MESSAGE}<b>NHS App Web Version</b> : $WEBVER<br>"
MESSAGE="${MESSAGE}<b>NHS App iOS Version</b> : $IOSVER<br>"
MESSAGE="${MESSAGE}<b>NHS App Android Version</b> : $ANDROIDVER<br>"
MESSAGE="${MESSAGE}<b>PROPOSED IMPLEMENTOR</b> : NHS App Team<br>"
MESSAGE="${MESSAGE}<b>PROPOSED START DATE</b> : $IMPLEMENTATIONDATE 10:00<br>"
MESSAGE="${MESSAGE}<b>PROPOSED END DATE</b> : $IMPLEMENTATIONDATE 11:00<br>"
MESSAGE="${MESSAGE}<b>IMPACT ASSESSMENT</b> : LOW<br>"
MESSAGE="${MESSAGE}<b>PROPOSED RISK MITIGATION</b> : Deployment is done to a cold namespace and tested before activating the new release. There is no service outage<br>"
MESSAGE="${MESSAGE}<b>TEST OUTCOME</b> : Tested as part of standard release process<br>"
MESSAGE="${MESSAGE}<b>IMPLEMENTATION PLAN</b> : Deploy to Cold Namespace, prove out deployment, route traffic to new namespace<br>"
MESSAGE="${MESSAGE}<b>BACKOUT PLAN</b> : Revert back to previous namespace<br>"
MESSAGE="${MESSAGE}<b>POST IMPLEMENTATION TESTING</b> : Sanity check of the release will be performed<br>"
MESSAGE="${MESSAGE}<b>SUCCESS CRITERIA</b> : Sanity check of the release will be performed, post deployment.<br>"

EMAIL="from:TeamCity@teamcity.dev.nonlive.nhsapp.service.nhs.uk\n"
EMAIL="${EMAIL}to:lee.gathercole@nhs.net\n"
EMAIL="${EMAIL}subject: $SUBJECT\n"
EMAIL="${EMAIL}${MESSAGE}"

TOEMAILADDRESS="matthew.smith48@nhs.net"
TOEMAILNAME="Matthew Smith"
FROMEMAILADDRESS="TeamCity@teamcity.dev.nonlive.nhsapp.service.nhs.uk"
FROMEMAILNAME="TeamCity"

JSON_FMT='{"personalizations" : [{"to": [{"email": "%s","name": "%s"}],"subject": "%s"}],"from": {"email": "%s", "name": "%s"},"reply_to": {"email": "%s", "name": "%s"},"content": [{"type": "text/html","value": "%s"}]}'

printf "$JSON_FMT" "$TOEMAILADDRESS" "$TOEMAILNAME" "$SUBJECT" "$FROMEMAILADDRESS" "$FROMEMAILNAME" "$FROMEMAILADDRESS" "$FROMEMAILNAME" "$MESSAGE" > $TAG.json

printf "$EMAIL" > $TAG.txt

info "Release email written to $TAG.txt"

