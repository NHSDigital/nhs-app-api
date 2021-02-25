#! /usr/bin/env bash
set -e

# Change current working directory to be the root of xamarin, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

MSBUILD_ARGS=()

function configure_key_store () {
  local ANDROID_KEY_STORE_LOCATION=${ANDROID_KEY_STORE_LOCATION:-~/.nhsonline/secrets/android-debug-store-file}
  local ANDROID_KEY_STORE_KEY_ALIAS=${ANDROID_KEY_STORE_KEY_ALIAS:-debug}

  if [ -z "$ANDROID_KEY_STORE_PASSWORD" ]; then
    local ANDROID_KEY_STORE_PASSWORD_FILE=${ANDROID_KEY_STORE_PASSWORD_FILE:-~/.nhsonline/secrets/android_keystore.password}
    ANDROID_KEY_STORE_PASSWORD=$(<"${ANDROID_KEY_STORE_PASSWORD_FILE}")
    export ANDROID_KEY_STORE_PASSWORD
  fi

  MSBUILD_ARGS+=("-p:AndroidKeyStore=True")
  MSBUILD_ARGS+=("-p:AndroidSigningKeyStore=${ANDROID_KEY_STORE_LOCATION}")
  MSBUILD_ARGS+=("-p:AndroidSigningStorePass=env:ANDROID_KEY_STORE_PASSWORD")
  MSBUILD_ARGS+=("-p:AndroidSigningKeyAlias=${ANDROID_KEY_STORE_KEY_ALIAS}")
  MSBUILD_ARGS+=("-p:AndroidSigningKeyPass=env:ANDROID_KEY_STORE_PASSWORD")
}

MSBUILD_ARGS+=("-p:Configuration=${CONFIGURATION}")
MSBUILD_ARGS+=("-p:Platform=Any CPU")
MSBUILD_ARGS+=("-restore")

configure_key_store

"${MSBUILD}" "${MSBUILD_ARGS[@]}" -t:SignAndroidPackage NHSOnline.App.Android/NHSOnline.App.Android.csproj

if [ -f "NHSOnline.App.Android/bin/Release/com.nhs.online.nhsonline.browserstack-Signed.apk" ]; then
  cp -f NHSOnline.App.Android/bin/Release/com.nhs.online.nhsonline.browserstack-Signed.apk ../xamarinintegrationtests/com.nhs.online.nhsonline.browserstack.apk
fi
