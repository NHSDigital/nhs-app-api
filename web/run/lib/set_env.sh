#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

DOCKER_ARGS=()

DOCKER_ARGS+=(-v "$REPO_ROOT/:/data/")
DOCKER_ARGS+=(-v web-node-modules:/data/web/node_modules)
DOCKER_ARGS+=(-v "${NPMRC_PATH}:/root/.npmrc")
