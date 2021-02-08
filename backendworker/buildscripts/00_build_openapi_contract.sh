#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

pull_docker_image "nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0"

DOCKER_ARGS=()
DOCKER_ARGS+=(--rm)
DOCKER_ARGS+=(-v "$REPO_ROOT/:/data/")
DOCKER_ARGS+=(-v "${NPMRC_PATH}:/root/.npmrc")

docker run \
  "${DOCKER_ARGS[@]}" \
  nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0 \
  ./backendworker/contracts/bundleContracts.sh
