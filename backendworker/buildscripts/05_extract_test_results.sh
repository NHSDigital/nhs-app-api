#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

# Remove any existing TestResults folder
[ ! -e TestResults ] || rm -r TestResults
[ ! -e coverage ] || rm -r coverage

docker cp \
  nhsonline-backend-test-run:/TestResults \
  TestResults || die "Failed to copy backend worker unit test results from container"

docker cp \
  nhsonline-backend-test-run:/coverage \
  coverage|| die "Failed to copy backend worker unit test coverage from container"
