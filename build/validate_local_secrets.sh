#!/bin/bash

set -e

MISSING_SECRETS=0
    
POSSIBLE_KEYBASE_PATHS=(/k /Volumes/Keybase /keybase)
for possible_keybase_path in ${POSSIBLE_KEYBASE_PATHS[*]}; do
    if [ -d "$possible_keybase_path" ]; then
        KEYBASE_PATH=$possible_keybase_path
    break
    fi
done

function validate_secret {
    SECRET_NAME="$1"
    SECRET_LOCAL_PATH=~/.nhsonline/secrets/$SECRET_NAME
   

    if [ ! -f "$SECRET_LOCAL_PATH" ]
    then
        mkdir -p ~/.nhsonline/secrets

        if [ -f "$KEYBASE_PATH/team/nhsonline/development_secrets/$SECRET_NAME" ]; then
            cp "$KEYBASE_PATH/team/nhsonline/development_secrets/$SECRET_NAME" "$SECRET_LOCAL_PATH"
        fi
    fi
    
    if [ ! -f "$SECRET_LOCAL_PATH" ]; then
        echo "Missing secret $SECRET_LOCAL_PATH"
        MISSING_SECRETS=$((MISSING_SECRETS+1))
    fi
}

validate_secret azure_notification_hub_key
validate_secret azure_notification_hub_key_bdd
validate_secret browserstack_accesskey
validate_secret microtest_client_cert.password
validate_secret microtest_client_cert.pfx
validate_secret organ_donation_cert.password
validate_secret organ_donation_cert.pfx
validate_secret session_encryption_key
validate_secret spine_client_cert.password
validate_secret spine_client_cert.pfx
validate_secret tpp_client_cert.password
validate_secret tpp_client_cert.pfx
validate_secret gp_lookup_api_key

exit $MISSING_SECRETS
