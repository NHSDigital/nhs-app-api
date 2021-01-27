#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

DEBUGMODE=${DEBUGMODE:-Release}
ENVIRONMENT=${ENVIRONMENT:-BrowserStack}
CONFIGURATION="${CONFIGURATION:-${ENVIRONMENT} - ${DEBUGMODE}}"
IPA_PACKAGE_DIR=${IPA_PACKAGE_DIR:-${REPO_ROOT}/xamarinintegrationtests}

MSBUILD=${MSBUILD:-$(command -v msbuild || true)}
if [[ $(uname -s) =~ ^MING.* ]]; then
  MSBUILD=${MSBUILD:-$(/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/Installer/vswhere.exe -latest -requires Microsoft.Component.MSBuild -find MSBuild/**/Bin/MSBuild.exe | tr -d '\r')}
elif [ ! -e "$HOME/.config/NuGet" ]; then
  ln -sfn ~/.nuget/NuGet ~/.config/
fi
