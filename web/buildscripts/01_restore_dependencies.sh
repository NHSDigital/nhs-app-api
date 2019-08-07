#!/bin/bash

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

docker build \
  --target=production_dependencies \
  --cache-from=local/nhsonline-web-production-dependencies \
  --tag local/nhsonline-web-production-dependencies \
  .

docker build \
  --target=build_dependencies \
  --cache-from=local/nhsonline-web-production-dependencies \
  --cache-from=local/nhsonline-web-build-dependencies \
  --tag local/nhsonline-web-build-dependencies \
  .

