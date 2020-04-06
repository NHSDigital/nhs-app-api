#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

docker build \
  --tag "local/nhsonline-dev-stubs:latest" \
  -f nhsonline-dev-stubs.Dockerfile \
  . || die "Failed to build dev stubs"
