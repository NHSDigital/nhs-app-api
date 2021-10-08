#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/functions_validation.sh
source "../buildscripts/lib/functions_validation.sh"

function validate_azure_notification_hub_key () {
  if [ ! -f ~/.nhsonline/secrets/azure_notification_hub_key_bdd ]; then
    [ -z "$AZURE_NOTIFICATION_HUBS__0__AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" ] && die "AZURE_NOTIFICATION_HUBS__0__AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY is not specified, this is required to run notifications integration tests"
    mkdir -p ~/.nhsonline/secrets
    echo "$AZURE_NOTIFICATION_HUBS__0__AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" >> ~/.nhsonline/secrets/azure_notification_hub_key_bdd
  fi
}
