#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --target=final \
  --build-arg=COMMIT_ID="$COMMIT_ID" \
  --build-arg=PROJECT_NAME=NHSOnline.Backend.CidApi \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-backendcidapi:${DOCKER_TAG:-latest}" \
  . || die "Failed to build ${DOCKER_REGISTRY:-local}/nhsonline-backendcidapi:${DOCKER_TAG:-latest}"

if [ -n "${BRANCH_TAG}" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-backendcidapi:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-backendcidapi:${BRANCH_TAG}" || die "Failed to tag ${DOCKER_REGISTRY}/nhsonline-backendcidapi:${BRANCH_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendcidapi:${DOCKER_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-backendcidapi:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-backendcidapi:${BRANCH_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-backendcidapi:${BRANCH_TAG}"
fi