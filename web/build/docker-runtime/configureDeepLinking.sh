#! /usr/bin/env bash
set -e

populateLinksTemplate() {
  name="$1"
  envSubCsv="$2"
  templateFile="$3"

  envsubst "${envSubCsv}" < "/app/.well-known/${templateFile}.template" > "/app/.well-known/${templateFile}";
  rm -f "/app/.well-known/${templateFile}.template";

  echo "Completed defining ${name} Links";
}

main() {
  echo "Begin Generating deep link configs"

  populateLinksTemplate "Universal" \
    '${WELL_KNOWN_ANDROID_PACKAGE_NAME},${WELL_KNOWN_ANDROID_FINGERPRINT}' \
    "assetlinks.json"

  populateLinksTemplate "App" '${WELL_KNOWN_IOS_APP_ID}' "apple-app-site-association"

  echo "Deep link configs generated"
}

main
