#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source buildscripts/lib/set_env.sh

# shellcheck source=lib/functions_logging.sh
source buildscripts/lib/functions_logging.sh

MISSING_SECRETS=0

POSSIBLE_KEYBASE_PATHS=(/k /Volumes/Keybase /keybase)
for possible_keybase_path in ${POSSIBLE_KEYBASE_PATHS[*]}; do
  if [ -d "$possible_keybase_path" ]; then
    KEYBASE_PATH=$possible_keybase_path
    break
  fi
done

if [ ! -d ~/.nhsonline/secrets ]; then
  mkdir -p ~/.nhsonline/secrets
fi

function validate_secret {
  SECRET_NAME="$1"
  SECRET_LOCAL_PATH=~/.nhsonline/secrets/$SECRET_NAME
  SECRET_KEYBASE_PATH="$KEYBASE_PATH/team/nhsonline/development_secrets/$SECRET_NAME"

  if [ "$SECRET_LOCAL_PATH" -ot "$SECRET_KEYBASE_PATH" ]; then
    echo "$SECRET_KEYBASE_PATH => $SECRET_LOCAL_PATH"
    cp "$SECRET_KEYBASE_PATH" "$SECRET_LOCAL_PATH"
  fi

  if [ ! -f "$SECRET_LOCAL_PATH" ]; then
    error "Missing secret $SECRET_LOCAL_PATH (from Keybase $SECRET_KEYBASE_PATH)"
    MISSING_SECRETS=$((MISSING_SECRETS+1))
  fi
}

validate_secret android-debug-store-file
validate_secret android-development.gradle
validate_secret android_keystore.password
validate_secret azure_notification_hub_key
validate_secret azure_notification_hub_key_bdd
validate_secret browserstack_accesskey
validate_secret gp_lookup_api_key
validate_secret microtest_client_cert.password
validate_secret microtest_client_cert.pfx
validate_secret nhsapp_api_key
validate_secret nhslogin_dev_private_key.pem
validate_secret nhslogin_dev_rsapass.password
validate_secret nhslogin_ext_private_key.pem
validate_secret nhslogin_ext_rsapass.password
validate_secret organ_donation_cert.password
validate_secret organ_donation_cert.pfx
validate_secret session_encryption_key
validate_secret spine_client_cert.password
validate_secret spine_client_cert.pfx
validate_secret tpp_client_cert.password
validate_secret tpp_client_cert.pfx
validate_secret qualtrics_api_key
validate_secret vision_client_cert.password
validate_secret vision_client_cert.pfx

exit $MISSING_SECRETS
