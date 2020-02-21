#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

TRANCHE_TAG=cosmos
TRANCHE_RUN_ADDITIONAL_ARGS+=(-v "//$HOME/.nhsonline/secrets/terms_conditions_cosmos_auth_key:/run/secrets/terms_conditions_cosmos_auth_key")
TRANCHE_RUN_SETUP="export TERMS_CONDITIONS_COSMOS_AUTH_KEY=\$(</run/secrets/terms_conditions_cosmos_auth_key);"

DOCKER_COMPOSE_FILES_TRANCHE+=('../docker/bddtests/docker-compose.cosmos.yml')

info "Running ${TESTS_NAME:-$TRANCHE_TAG} tests"

generate_tags $TRANCHE_TAG
validate_terms_conditions_cosmos_auth_key

# shellcheck source=lib/run_tests.sh
source "buildscripts/lib/run_tests.sh"
