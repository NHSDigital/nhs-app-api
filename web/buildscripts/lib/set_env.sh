#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

export DOCKER_IMAGE_WEB_BUILD="nhsapp.azurecr.io/nhsonline-nodejs-build:node14-1.0"
export DOCKER_IMAGE_WEB_RUNTIME="nhsapp.azurecr.io/nhsonline-nginx:v1.18.0"
export DOCKER_IMAGE_HAWKEYE="hawkeyesec/scanner-cli:latest"
