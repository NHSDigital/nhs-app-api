#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# Remove any existing TestResults folder
rm -r TestResults 2>/dev/null
rm -r coverage 2>/dev/null

docker cp \
  nhsonline-backend-test-run:/TestResults \
  TestResults

docker cp \
  nhsonline-backend-test-run:/coverage \
  coverage
