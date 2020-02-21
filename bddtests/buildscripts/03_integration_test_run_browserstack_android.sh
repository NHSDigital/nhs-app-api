#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

TRANCHE_TAG=nativesmoketest

TRANCHE_RUN_GRADLE_ARGS+=("-Dappium.platformName=ANDROID")

BROWSER=browserstack_android
BROWSERSTACK_DEVICE_NAME=${BROWSERSTACK_DEVICE_NAME:-Google Pixel 2}
BROWSERSTACK_OS_VERSION=${BROWSERSTACK_OS_VERSION:-8.0}
BROWSERSTACK_CUSTOM_ID=${BROWSERSTACK_CUSTOM_ID:-${HOSTNAME}-android}

EXCLUDE_TAGS=(ios)

validate_browserstack_environment "$NATIVE_APP_PATH_ANDROID"

info "Running ${TESTS_NAME:-$TRANCHE_TAG} tests"

# shellcheck source=lib/run_browserstack.sh
source "buildscripts/lib/run_browserstack.sh"
