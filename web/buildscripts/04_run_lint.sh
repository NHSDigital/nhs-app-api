#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=lint \
  --cache-from=local/nhsonline-web-build-dependencies \
  --tag local/nhsonline-web-lint \
  --build-arg COMMIT_ID="$(git rev-parse --short HEAD)" \
  --build-arg APP_VERSION_TAG="${BRANCH_TAG}" \
  . || die "Web linting failed"
