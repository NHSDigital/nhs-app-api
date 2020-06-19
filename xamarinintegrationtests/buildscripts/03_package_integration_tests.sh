#! /usr/bin/bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=integrationtests \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}" \
  . || die "Failed to build ${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}"
