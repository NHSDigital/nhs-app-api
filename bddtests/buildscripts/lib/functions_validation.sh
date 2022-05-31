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

function validate_cosmos_sql_api_connection_string () {
  if [ ! -f ~/.nhsonline/secrets/cosmos_sql_api_connection_string_bdd ]; then
    [ -z "$COSMOS_SQL_API_CONNECTION_STRING" ] && die "COSMOS_SQL_API_CONNECTION_STRING is not specified, this is required to run integration tests"
    mkdir -p ~/.nhsonline/secrets
    echo "$COSMOS_SQL_API_CONNECTION_STRING" >> ~/.nhsonline/secrets/cosmos_sql_api_connection_string_bdd
  fi
}
