#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=../../buildscripts/lib/functions_logging.sh
source "../buildscripts/lib/functions_logging.sh"

COMMIT_ID=$(git rev-parse --short HEAD)

if [ 1 -eq "$(docker ps -a | grep -c nhsonline-backend-test-run)" ]
then
  docker rm nhsonline-backend-test-run
fi

docker run \
  --name nhsonline-backend-test-run \
  "local/backend-build:$COMMIT_ID" \
  bash -c "
    mkdir /coverage; \
    dotnet test \
    -c Release \
    --no-build \
    --results-directory TestResults \
    --logger:trx \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=cobertura; \
    test_run_result=\$?; \
    mkdir /TestResults; \
    index=1; \
    for trx in */TestResults/*.trx; do \
      cp \$trx /TestResults/\$index.trx; \
      ((index+=1)); \
    done; \
    index=1; \
    for coverage in */coverage.cobertura.xml; do \
      cp \"\$coverage\" /coverage/backend-\$index-cobertura.xml; \
      ((index+=1)); \
    done;
    exit \$test_run_result;" || die "Backend worker unit tests failed"
