#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

NUGET_CFG_PATH=""

function pull_images() {
  pull_docker_image "$DOCKER_IMAGE_DOTNET_BUILD"
}

function validate_nuget_config() {
  NUGET_CFG_PATH="${HOME}/.nuget/NuGet/NuGet.Config"

  if [[ $(uname -s) =~ ^MING.* ]]; then
    NUGET_CFG_PATH="${APPDATA}/NuGet/NuGet.Config"
  fi

  validate_config_file_present "${NUGET_CFG_PATH}"
}

function restore_packages() {
  docker build \
  --target=dependencies \
  --secret "id=nuget,src=${NUGET_CFG_PATH}" \
  --build-arg BASE_IMAGE="${DOCKER_IMAGE_DOTNET_BUILD}" \
  . || die "Failed to restore packages"
}

function main() {
  pull_images

  validate_nuget_config
  restore_packages
}

main
