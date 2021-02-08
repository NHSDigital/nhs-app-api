#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

validate_maven_settings

docker build \
  --tag "local/nhsonline-dev-stubs:latest" \
  --secret "id=maven,src=${MVN_CFG_PATH}" \
  -f nhsonline-dev-stubs.Dockerfile \
  . || die "Failed to build dev stubs"
