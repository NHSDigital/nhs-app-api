#! /usr/bin/env bash
set -ex

# Change current working directory to be the root of xamarin, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

# shellcheck source=lib/set_android_env.sh
source "buildscripts/lib/set_android_env.sh"

"${MSBUILD}" -p:JavaSdkDirectory="${JAVA_HOME}" "${MSBUILD_ARGS_ANDROID[@]}" -t:SignAndroidPackage NHSOnline.App.Android/NHSOnline.App.Android.csproj

if [ -f "NHSOnline.App.Android/bin/Release/com.nhs.online.nhsonline.browserstack-Signed.apk" ]; then
  cp -f NHSOnline.App.Android/bin/Release/com.nhs.online.nhsonline.browserstack-Signed.apk ../xamarinintegrationtests/com.nhs.online.nhsonline.browserstack.apk
fi
