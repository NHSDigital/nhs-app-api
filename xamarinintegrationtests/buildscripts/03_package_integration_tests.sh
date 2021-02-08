#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

docker build \
  --target=integrationtests \
  --build-arg BASE_IMAGE="${DOCKER_IMAGE_DOTNET_BUILD}" \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}" \
  . || die "Failed to build ${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}"
