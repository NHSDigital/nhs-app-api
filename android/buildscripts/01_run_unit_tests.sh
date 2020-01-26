#!/bin/bash

set -e

# Change current working directory to be the root of android, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

GRADLE_ARGS+=("testDebugUnitTest")

# shellcheck source=lib/run_gradle.sh
source "buildscripts/lib/run_gradle.sh"
