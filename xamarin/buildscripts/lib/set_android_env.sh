#! /usr/bin/env bash

# shellcheck source=set_env.sh
source "buildscripts/lib/set_env.sh"

MSBUILD_ARGS_ANDROID=()

function configure_key_store () {
  local ANDROID_KEY_STORE_LOCATION=${ANDROID_KEY_STORE_LOCATION:-~/.nhsonline/secrets/android-debug-store-file}
  local ANDROID_KEY_STORE_KEY_ALIAS=${ANDROID_KEY_STORE_KEY_ALIAS:-debug}

  if [ -z "$ANDROID_KEY_STORE_PASSWORD" ]; then
    local ANDROID_KEY_STORE_PASSWORD_FILE=${ANDROID_KEY_STORE_PASSWORD_FILE:-~/.nhsonline/secrets/android_keystore.password}
    ANDROID_KEY_STORE_PASSWORD=$(<"${ANDROID_KEY_STORE_PASSWORD_FILE}")
    export ANDROID_KEY_STORE_PASSWORD
  fi

  MSBUILD_ARGS_ANDROID+=("-p:AndroidKeyStore=True")
  MSBUILD_ARGS_ANDROID+=("-p:AndroidSigningKeyStore=${ANDROID_KEY_STORE_LOCATION}")
  MSBUILD_ARGS_ANDROID+=("-p:AndroidSigningStorePass=env:ANDROID_KEY_STORE_PASSWORD")
  MSBUILD_ARGS_ANDROID+=("-p:AndroidSigningKeyAlias=${ANDROID_KEY_STORE_KEY_ALIAS}")
  MSBUILD_ARGS_ANDROID+=("-p:AndroidSigningKeyPass=env:ANDROID_KEY_STORE_PASSWORD")
}

MSBUILD_ARGS_ANDROID+=("-p:NhsAppVersionName=${NATIVE_VERSION_NUMBER}")
MSBUILD_ARGS_ANDROID+=("-p:Configuration=${CONFIGURATION}")
MSBUILD_ARGS_ANDROID+=("-p:Platform=Any CPU")
MSBUILD_ARGS_ANDROID+=("-restore")

configure_key_store
