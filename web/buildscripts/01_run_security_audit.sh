#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

pull_docker_image "${DOCKER_IMAGE_WEB_BUILD}"

docker build \
  --target=security_audit \
  --build-arg "DOCKER_IMAGE_WEB_BUILD=${DOCKER_IMAGE_WEB_BUILD}" \
  --build-arg "DOCKER_IMAGE_WEB_RUNTIME=${DOCKER_IMAGE_WEB_RUNTIME}" \
  --secret "id=npmrc,src=${NPMRC_PATH}" \
  . || die "Security audit failed"
