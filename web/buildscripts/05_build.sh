#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

docker build \
  --target=build \
  --cache-from=local/nhsonline-web-lint \
  --tag local/nhsonline-web-build \
  --build-arg "DOCKER_IMAGE_WEB_BUILD=${DOCKER_IMAGE_WEB_BUILD}" \
  --build-arg "DOCKER_IMAGE_WEB_RUNTIME=${DOCKER_IMAGE_WEB_RUNTIME}" \
  --build-arg COMMIT_ID="$(git rev-parse --short HEAD)" \
  --build-arg APP_VERSION_TAG="$BRANCH_TAG" \
  . || die "Failed to build web"
