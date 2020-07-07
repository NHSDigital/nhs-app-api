#! /usr/bin/env bash

DEBUGMODE=${DEBUGMODE:-Release}
ENVIRONMENT=${ENVIRONMENT:-BrowserStack}
CONFIGURATION="${CONFIGURATION:-${ENVIRONMENT} - ${DEBUGMODE}}"

MSBUILD=${MSBUILD:-$(command -v msbuild || true)}
if [[ $(uname -s) =~ ^MING.* ]]; then
  MSBUILD=${MSBUILD:-$(/c/Program\ Files\ \(x86\)/Microsoft\ Visual\ Studio/Installer/vswhere.exe -latest -requires Microsoft.Component.MSBuild -find MSBuild/**/Bin/MSBuild.exe | tr -d '\r')}
else
  ln -sf ~/.nuget/NuGet ~/.config/Nuget
fi
