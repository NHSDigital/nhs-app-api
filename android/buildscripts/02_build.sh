#! /usr/bin/env bash
set -e

# Change current working directory to be the root of android, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

TARGET=${TARGET:-browserstack}

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"
# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

validate_maven_settings

SCRIPT_NAME="build"

if [[ $(uname -s) =~ ^MING.* ]]; then
  LOCAL_HOME_DIRECTORY="${USERPROFILE}"
else
  LOCAL_HOME_DIRECTORY="$HOME"
fi

ANDROID_DEBUG_KEY_STORE_LOCATION=${ANDROID_DEBUG_KEY_STORE_LOCATION:-${LOCAL_HOME_DIRECTORY}/.nhsonline/secrets/android-debug-store-file}
if [ -z "$ANDROID_DEBUG_KEY_STORE_PASSWORD" ] && [ -f "${LOCAL_HOME_DIRECTORY}/.nhsonline/secrets/android_keystore.password" ]; then
  ANDROID_DEBUG_KEY_STORE_PASSWORD=$(<${LOCAL_HOME_DIRECTORY}/.nhsonline/secrets/android_keystore.password)
fi
# set the default value for the key store and key location if building locally.

if [ -n "$ANDROID_KEY_STORE_PASSWORD" ] && [ -n "$ANDROID_KEY_STORE_LOCATION" ]; then
  DOCKER_ARGS+=(-v "$ANDROID_KEY_STORE_LOCATION:/secret/AndroidKeyStore")
  GRADLE_ARGS+=("-Prelease_store_password='$ANDROID_KEY_STORE_PASSWORD'" "-Prelease_store_location='/secret/AndroidKeyStore'")
elif [ -n "$ANDROID_DEBUG_KEY_STORE_PASSWORD" ] && [ -n "$ANDROID_DEBUG_KEY_STORE_LOCATION" ]; then
  DOCKER_ARGS+=(-v "$ANDROID_DEBUG_KEY_STORE_LOCATION:/secret/AndroidDebugKeyStore")
  GRADLE_ARGS=("${GRADLE_ARGS[@]} -Pdebug_store_password='$ANDROID_DEBUG_KEY_STORE_PASSWORD' -Pdebug_store_location='/secret/AndroidDebugKeyStore'")
fi

GRADLE_ARGS+=("assemble$TARGET")

# shellcheck source=lib/run_gradle.sh
source "buildscripts/lib/run_gradle.sh"
