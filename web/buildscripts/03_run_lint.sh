#!/bin/bash

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

docker run \
  local/nhsonline-web-build \
  npm run lint-raw