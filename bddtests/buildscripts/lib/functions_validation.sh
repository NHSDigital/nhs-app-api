#!/bin/bash

function validate_azure_notification_hub_key () {
  if [ ! -f ~/.nhsonline/secrets/azure_notification_hub_key_bdd ]; then
    [ -z "$AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" ] && die "AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY is not specified, this is required to run notifications integration tests"
    mkdir -p ~/.nhsonline/secrets
    echo "$AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY" >> ~/.nhsonline/secrets/azure_notification_hub_key_bdd
  fi
}

function validate_terms_conditions_cosmos_auth_key () {
  if [ ! -f ~/.nhsonline/secrets/terms_conditions_cosmos_auth_key ]; then
    [ -z "$TERMS_CONDITIONS_COSMOS_AUTH_KEY" ] && die "TERMS_CONDITIONS_COSMOS_AUTH_KEY is not specified, this is required to run cosmos integration tests"
    mkdir -p ~/.nhsonline/secrets
    echo "$TERMS_CONDITIONS_COSMOS_AUTH_KEY" >> ~/.nhsonline/secrets/terms_conditions_cosmos_auth_key
  fi
}
