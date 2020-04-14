#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=build \
  --cache-from=local/nhsonline-web-lint \
  --tag local/nhsonline-web-build \
  --build-arg COMMIT_ID="$(git rev-parse --short HEAD)" \
  --build-arg APP_VERSION_TAG="$BRANCH_TAG" \
  . || die "Failed to build web"
