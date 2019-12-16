#!/bin/bash

set -e

MISSING_SECRETS=0

function validate_secret {
    SECRET_NAME="$1"
    SECRET_LOCAL_PATH=~/.nhsonline/secrets/$SECRET_NAME

    if [ ! -f "$SECRET_LOCAL_PATH" ]
    then
        mkdir -p ~/.nhsonline/secrets

        if [ -f "/k/team/nhsonline/development_secrets/$SECRET_NAME" ]; then
            cp "/k/team/nhsonline/development_secrets/$SECRET_NAME" "$SECRET_LOCAL_PATH"
        elif [ -f "/keybase/team/nhsonline/development_secrets/$SECRET_NAME" ]; then
            cp "/keybase/team/nhsonline/development_secrets/$SECRET_NAME" "$SECRET_LOCAL_PATH"
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

exit $MISSING_SECRETS
