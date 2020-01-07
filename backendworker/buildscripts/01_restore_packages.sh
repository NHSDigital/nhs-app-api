#!/bin/bash

set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

docker pull nhsapp.azurecr.io/nhsonline-dotnetcore-build:latest
docker pull nhsapp.azurecr.io/nhsonline-aspdotnetcore-runtime:2.1

docker build \
  --target=dependencies \
  .