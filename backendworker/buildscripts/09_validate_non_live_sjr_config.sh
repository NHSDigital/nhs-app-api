#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=validated \
  --build-arg="SJR_IMAGE=${DOCKER_REGISTRY:-local}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG:-latest}" \
  configurations || die "Failed to validate non live SJR configuration"