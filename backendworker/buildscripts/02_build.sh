#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

# Cleanup old build containers and images
for IMAGE in $(docker images --filter=reference=local/backend-build --format='{{.Repository}}:{{.Tag}}'); do
  for CONTAINER in $(docker ps -aq --filter=ancestor="$IMAGE"); do docker rm --force "$CONTAINER"; done
  docker rmi --force "$IMAGE"
done

COMMIT_ID=$(git rev-parse --short HEAD)

docker build \
  --target=built \
  --build-arg "DOCKER_IMAGE_BACKEND_BUILD=${DOCKER_IMAGE_BACKEND_BUILD}" \
  --build-arg "DOCKER_IMAGE_BACKEND_RUNTIME=${DOCKER_IMAGE_BACKEND_RUNTIME}" \
  --build-arg=COMMIT_ID="$COMMIT_ID" \
  --tag "local/backend-build:$COMMIT_ID" \
  . || die "Failed to build backend worker"