#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker pull nhsapp.azurecr.io/nhsonline-dotnetcore-build:latest || die "Failed to pull nhsapp.azurecr.io/nhsonline-dotnetcore-build:latest"
docker pull nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:2.1 || die "Failed to pull nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:2.1"

docker build \
  --target=dependencies \
  . || die "Failed to restore backend worker packages"