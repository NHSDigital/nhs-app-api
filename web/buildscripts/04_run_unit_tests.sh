#!/bin/bash

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# Delete any old container hanging around
docker container rm nhsonline-web-test-run 2>/dev/null || true

docker run \
  --name nhsonline-web-test-run \
  local/nhsonline-web-build \
  npm run test-jest -- --runInBand

test_run_result=$?

if [ -z "$TEAMCITY_VERSION" ]; then
  exit
fi;

if [ -z "$TF_BUILD" ]; then
  exit $test_run_result
fi;

if [ $test_run_result -ne 0 ]; then
  echo "##vso[task.logissue type=error]Web Unit Tests Failed"
  echo "##vso[task.complete result=Failed;]Web Unit Tests Failed"
fi;
