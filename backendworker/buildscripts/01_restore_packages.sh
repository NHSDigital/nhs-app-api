#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

# shellcheck source=../../buildscripts/lib/functions_validation.sh
source "../buildscripts/lib/functions_validation.sh"

NUGET_CFG_PATH=""

export DOCKER_BUILDKIT=1

function restore_packages() {
  docker build \
  --target=dependencies \
  --secret "id=nuget,src=${NUGET_CFG_PATH}" \
  . || die "Failed to restore backend worker packages"
}

function validate_nuget_config() {
  NUGET_CFG_PATH="${HOME}/.nuget/NuGet/NuGet.Config"

  if [[ $(uname -s) =~ ^MING.* ]]; then
    NUGET_CFG_PATH="${APPDATA}/NuGet/NuGet.Config"
  fi

  validate_config_file_present "${NUGET_CFG_PATH}"
}

function pull_images() {
  docker pull nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1 \
    || die "Failed to pull nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1"

  docker pull nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:3.1 \
    || die "Failed to pull nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:3.1"
}

function main() {
  pull_images

  validate_nuget_config
  restore_packages
}

main
