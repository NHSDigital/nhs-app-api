#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=release \
  --cache-from=local/nhsonline-web-production-dependencies \
  --cache-from=local/nhsonline-web-build \
  --build-arg COMMIT_ID="$(git rev-parse --short HEAD)" \
  --build-arg APP_VERSION_TAG="${BRANCH_TAG}" \
  --tag="${DOCKER_REGISTRY:-local}/nhsonline-web:${DOCKER_TAG:-latest}" \
  . || die "Failed to build web docker image"

if [ -n "${BRANCH_TAG}" ]
then
  docker tag "${DOCKER_REGISTRY}/nhsonline-web:${DOCKER_TAG}" "${DOCKER_REGISTRY}/nhsonline-web:$BRANCH_TAG" || die "Failed to tag web docker image"
  docker push "${DOCKER_REGISTRY}/nhsonline-web:${DOCKER_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-web:${DOCKER_TAG}"
  docker push "${DOCKER_REGISTRY}/nhsonline-web:${BRANCH_TAG}" || die "Failed to push ${DOCKER_REGISTRY}/nhsonline-web:${BRANCH_TAG}"
fi
