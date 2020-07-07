#! /usr/bin/env bash
set -e

# Change current working directory to be the root of xamarin, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

MSBUILD_ARGS=()

MSBUILD_ARGS+=("-p:Configuration=${CONFIGURATION}")
MSBUILD_ARGS+=("-p:Platform=iPhone")
MSBUILD_ARGS+=("-p:BuildIpa=true")
MSBUILD_ARGS+=("-restore")
MSBUILD_ARGS+=("-p:IpaPackageDir=${IPA_PACKAGE_DIR}")

if [ -n "${APPLE_CERTIFICATE_SIGNING_IDENTITY}" ]; then
  MSBUILD_ARGS+=("-p:Codesignkey=${APPLE_CERTIFICATE_SIGNING_IDENTITY}")
fi
if [ -n "${APPLE_PROV_PROFILE_UUID}" ]; then
  MSBUILD_ARGS+=("-p:CodesignProvision=${APPLE_PROV_PROFILE_UUID}")
fi

if [ -x "remote_mac_config.sh" ]; then
  source "remote_mac_config.sh"
elif [[ $(uname -s) =~ ^MING.* ]]; then
  info 'To build on Windows using a mac build server create remote_mac_config.sh containing:
#! /usr/bin/env bash

MSBUILD_ARGS+=("-p:ServerUser=[macusername]")
MSBUILD_ARGS+=("-p:ServerAddress=[machostname]")'
  exit
fi

"${MSBUILD}" "${MSBUILD_ARGS[@]}" NHSOnline.App.iOS/NHSOnline.App.iOS.csproj
