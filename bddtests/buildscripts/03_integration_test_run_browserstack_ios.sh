#! /usr/bin/env bash
set -e

# Change current working directory to be the root of bddtests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

TRANCHE_TAG=${TRANCHE_TAG:-native}

TRANCHE_RUN_GRADLE_ARGS+=("-Dappium.platformName=iOS")

BROWSER=browserstack_ios
BROWSERSTACK_DEVICE_NAME=${BROWSERSTACK_DEVICE_NAME:-iPhone 8}
BROWSERSTACK_OS_VERSION=${BROWSERSTACK_OS_VERSION:-12.1}
BROWSERSTACK_CUSTOM_ID=${BROWSERSTACK_CUSTOM_ID:-${HOSTNAME}-ios}

export EXCLUDE_TAGS=(android)

validate_browserstack_environment "NATIVE_APP_PATH_IOS"

setup_browserstack_environment_variables

generate_browserstack_local_identifier

generate_tags_native "$TRANCHE_TAG"

upload_app_to_browserstack

set_browserstack_additional_args

# shellcheck source=lib/run_tests.sh
source "buildscripts/lib/run_tests.sh"
