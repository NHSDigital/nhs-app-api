#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/functions_validation.sh
source "../buildscripts/lib/functions_validation.sh"

function validate_azure_notification_hub_key () {
  if [ ! -f ~/.nhsonline/secrets/azure_notification_hub_key_bdd ]; then
    [ -z "$AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" ] && die "AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY is not specified, this is required to run notifications integration tests"
    mkdir -p ~/.nhsonline/secrets
    echo "$AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" >> ~/.nhsonline/secrets/azure_notification_hub_key_bdd
  fi
}

function validate_browserstack_environment () {
  export NATIVE_APP_PATH="${!1}"

  if [ -z "$BROWSERSTACK_ACCESS_KEY" ] && [ -f ~/.nhsonline/secrets/browserstack_accesskey ]; then
    BROWSERSTACK_ACCESS_KEY=$(<~/.nhsonline/secrets/browserstack_accesskey)
    export BROWSERSTACK_ACCESS_KEY
  fi

  [ -n "$BROWSERSTACK_ACCESS_KEY" ] || die "BROWSERSTACK_ACCESS_KEY is not specified, this is required to run native integration tests"
  [ -f "$NATIVE_APP_PATH" ] || die "Native app ($1=$NATIVE_APP_PATH) does not exist, this is required to run native integration tests"
}
