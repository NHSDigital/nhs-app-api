#!/bin/bash

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

COMMIT_ID=$(git rev-parse --short HEAD)

if [ 1 -eq $(docker ps -a | grep nhsonline-backend-test-run | wc -l) ]
then
  docker rm nhsonline-backend-test-run
fi

RUN_UNIT_TESTS_COMMAND='
  mkdir /coverage; \
  dotnet test \
  --no-restore \
  --results-directory TestResults \
  --logger:trx \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:Parallel=true \
  /p:CopyLocalLockFileAssemblies=true; \
  test_run_result=$?; \
  mkdir /TestResults; \
  index=1; \
  for trx in */TestResults/*.trx; do \
    cp $trx /TestResults/$index.trx; \
    ((index+=1)); \
  done; \
  index=1; \
  for coverage in */coverage.cobertura.xml; do \
    sed "#\s*<source>.*</source>#<source>backendworker/</source>#" $coverage > /coverage/$index.coverage.cobertura.xml; \
    ((index+=1)); \
  done; '

if [ -z "$TF_BUILD" ]; then
  RUN_UNIT_TESTS_COMMAND+='exit $test_run_result;'
else
  RUN_UNIT_TESTS_COMMAND+='if [ $test_run_result -ne 0 ]; then \
  echo "##vso[task.logissue type=error]Backend Unit Tests Failed"; \
  echo "##vso[task.complete result=Failed;]Backend Unit Tests Failed"; \
fi;'
fi;

docker run \
  --memory="1g" \
  --cpus=1 \
  --name nhsonline-backend-test-run \
  "local/backend-build:$COMMIT_ID" \
  /bin/bash -c "$RUN_UNIT_TESTS_COMMAND"

test_run_result=$?

if [ -z "$TEAMCITY_VERSION" ]; then
  exit
fi;

exit $test_run_result
