#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

export DOCKER_IMAGE_BACKEND_BUILD="nhsapp.azurecr.io/nhsonline-dotnetcore-build:3.1"
export DOCKER_IMAGE_BACKEND_RUNTIME="nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:3.1"
