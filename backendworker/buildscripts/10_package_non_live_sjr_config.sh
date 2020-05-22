#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=final \
  --build-arg="SJR_IMAGE=${DOCKER_REGISTRY:-local}/nhsonline-backendservicejourneyrulesapi:${DOCKER_TAG:-latest}" \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-service-journey-dev-config:${DOCKER_TAG:-latest}" \
  configurations || die "Failed to build ${DOCKER_REGISTRY:-local}/nhsonline-service-journey-dev-config:${DOCKER_TAG:-latest}"

if [ -n "${BRANCH_TAG}" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${BRANCH_TAG}" || die "Failed to tag ${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${BRANCH_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${DOCKER_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${BRANCH_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-service-journey-dev-config:${BRANCH_TAG}"
fi
