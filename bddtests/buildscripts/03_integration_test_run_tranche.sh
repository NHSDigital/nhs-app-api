#!/bin/bash

set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

TRANCHE_TAG=${TRANCHE_TAG:-smoketest}
DOCKER_TEST_RUN_IMAGES+=("$DOCKER_IMAGE_CHROME")

info "Running ${TESTS_NAME:-$TRANCHE_TAG} tests"

generate_tags "$TRANCHE_TAG"

# shellcheck source=lib/run_tests.sh
source "buildscripts/lib/run_tests.sh"
