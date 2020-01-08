#!/bin/bash

set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

TRANCHE_TAG=throttling
DOCKER_TEST_RUN_IMAGES+=("$DOCKER_IMAGE_CHROME")
DOCKER_COMPOSE_FILES_TRANCHE+=('../docker/bddtests/docker-compose.throttling.yml')

info "Running ${TESTS_NAME:-$TRANCHE_TAG} tests"

generate_tags $TRANCHE_TAG

# shellcheck source=lib/run_tests.sh
source "buildscripts/lib/run_tests.sh"
