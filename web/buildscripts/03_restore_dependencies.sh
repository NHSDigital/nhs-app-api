#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

# shellcheck source=./lib/functions.sh
source "buildscripts/lib/functions.sh"

function restore_build_dependencies() {
  docker build \
    --progress=plain \
    --target=build_dependencies \
    --cache-from=local/nhsonline-web-production-dependencies \
    --cache-from=local/nhsonline-web-build-dependencies \
    --tag local/nhsonline-web-build-dependencies \
    --secret "id=npmrc,src=${NPMRC_PATH}" \
    . || die "Failed to restore build dependencies"
}

function restore_production_dependencies() {
  docker build \
    --progress=plain \
    --target=production_dependencies \
    --cache-from=local/nhsonline-web-production-dependencies \
    --tag local/nhsonline-web-production-dependencies \
    --secret "id=npmrc,src=${NPMRC_PATH}" \
    . || die "Failed to restore production dependencies"
}

function main() {
  validate_npm_settings

  restore_production_dependencies
  restore_build_dependencies
}

main
