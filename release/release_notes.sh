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


GITLOG=`git log --pretty=format:"%s" $VERSION_BELOW..$TAG`
IMPLEMENTATIONDATE=`date -dnext-monday +%d-%m-%Y`

SUBJECT="Release - $TAG"
MESSAGE="Change Notification for NHS APP\n"
MESSAGE="This change is approved by Solution Assurance and approval has been received from all necessary areas\n"
MESSAGE="$MESSAGE SERVICE AFFECTED : NHS App\n"
MESSAGE="$MESSAGE PRIORITY : Standard Change\n"
MESSAGE="$MESSAGE DESCRIPTION : Standard Feature Release for NHS App\n"
MESSAGE="$MESSAGE $GITLOG\n"
MESSAGE="$MESSAGE PRIMARY CI : NHS App\n"
MESSAGE="$MESSAGE PROPOSED IMPLEMENTOR : NHS App Team\n"
MESSAGE="$MESSAGE PROPOSED START DATE : $IMPLEMENTATIONDATE 10:00\n"
MESSAGE="$MESSAGE PROPOSED END DATE : $IMPLEMENTATIONDATE 11:00\n"
MESSAGE="$MESSAGE IMPACT ASSESSMENT : LOW\n"
MESSAGE="$MESSAGE PROPOSED RISK MITIGATION : Deployment is done to a cold namespace and tested before activating the new release. There is no service outage\n"
MESSAGE="$MESSAGE TEST OUTCOME : Tested as part of\n"
MESSAGE="$MESSAGE IMPLEMENTATION PLAN : Deploy to Cold Namespace, proove out deployment, route traffic to new namespace\n"
MESSAGE="$MESSAGE BACKOUT PLAN : Revert back to previous namespace\n"
MESSAGE="$MESSAGE POST IMPLEMENTATION TESTING : Sanity check of the release will be performed\n"
MESSAGE="$MESSAGE SUCCESS CRITERIA : Sanity check of the release will be performed, post deployment.\n"

EMAIL="from:TeamCity@teamcity.dev.nonlive.nhsapp.service.nhs.uk\n"
EMAIL="$EMAIL to:lee.gathercole@nhs.net\n"
EMAIL="$EMAIL subject: $SUBJECT\n"
EMAIL="$EMAIL $MESSAGE"

printf "$EMAIL" > $TAG.txt

info "Release email written to $TAG.txt"

