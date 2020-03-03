#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --target=production_dependencies \
  --cache-from=local/nhsonline-web-production-dependencies \
  --tag local/nhsonline-web-production-dependencies \
  . || die "Failed to restore production dependencies"

docker build \
  --target=build_dependencies \
  --cache-from=local/nhsonline-web-production-dependencies \
  --cache-from=local/nhsonline-web-build-dependencies \
  --tag local/nhsonline-web-build-dependencies \
  . || die "Failed to restore build dependencies"
