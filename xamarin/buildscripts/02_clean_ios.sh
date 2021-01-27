#! /usr/bin/env bash
set -e

# Change current working directory to be the root of xamarin, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

MSBUILD_ARGS=()

MSBUILD_ARGS+=("-p:Configuration=${ENVIRONMENT} - ${DEBUGMODE}")
MSBUILD_ARGS+=("-p:Platform=iPhone")

"${MSBUILD}" "${MSBUILD_ARGS[@]}" -t:Clean
