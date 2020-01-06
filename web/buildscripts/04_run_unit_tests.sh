#!/bin/bash

set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

if [ 1 -eq "$(docker ps -a | grep -c nhsonline-web-test-run)" ]
then
  docker rm nhsonline-web-test-run
fi

set +e

docker run \
  --name nhsonline-web-test-run \
  local/nhsonline-web-build \
  npm run test-jest

test_run_result=$?

set -e

if [ -n "$TEAMCITY_VERSION" ]; then
  exit
fi;

if [ -z "$TF_BUILD" ]; then
  exit $test_run_result
fi;

if [ $test_run_result -ne 0 ]; then
  echo "##vso[task.logissue type=error]Web Unit Tests Failed"
  echo "##vso[task.complete result=Failed;]Web Unit Tests Failed"
fi;
