#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

DOCKER_ARGS=()
DOCKER_ARGS+=(--name "int_test_test_runner")
DOCKER_ARGS+=(--network "${DOCKER_NETWORK}")
DOCKER_ARGS+=(--env "Chrome__Arguments__headless=true" --env "NOMINATED_PHARMACY_ENABLED=true")
DOCKER_ARGS+=(-v "//var/run/docker.sock:/var/run/docker.sock")

set_int_test_config_from_environment

setup_browserstack_environment

cleanup_docker_containers

set_docker_compose_files_args

pull_docker_images

start_services_under_test

set +e

if [ "$USE_APP_UPGRADE_CONFIG" == 'True' ]
then
  TEST_FILTER="TestCategory=NhsAppUpgradeTest"
elif [ "$CANARY_RUN" == 'True' ]
then
  TEST_FILTER="TestCategory=NhsAppCanaryTest"
elif [ "$FLIPBOOK_RUN" == 'True' ]
then
  rm -rf flipbookgeneration/flipbook/
  TEST_FILTER="TestCategory=NhsAppFlipbookTest"
elif [ "$FLAKY_RUN" == 'True' ]
then
  TEST_FILTER="TestCategory=NhsAppFlakyTest"
else
  TEST_FILTER='"TestCategory!=NhsAppUpgradeTest&TestCategory!=NhsAppCanaryTest&TestCategory!=NhsAppFlipbookTest&TestCategory!=NhsAppFlakyTest"'
fi
info "Using test filter ${TEST_FILTER}"

docker run \
  "${DOCKER_ARGS[@]}" \
  "${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}" bash -c "
    dotnet test --filter ${TEST_FILTER} --no-build -c Release -r /src/TestResults --logger trx NHSOnline.IntegrationTests/NHSOnline.IntegrationTests.csproj;
    TESTS_EXIT_CODE=\$? ;
    rm -Rf /src/TestResults/Deploy* ;
    exit \$TESTS_EXIT_CODE"

TESTS_EXIT_CODE=$?

set -e

docker cp int_test_test_runner:/src/TestResults/. TestResults

if [ "$FLIPBOOK_RUN" == 'True' ]
  then  docker cp int_test_test_runner:/src/flipbook/. flipbookgeneration/flipbook
fi

stop_services_under_test

fetch_container_logs

destroy_services_under_test

## We want to still produce the flipbook for the passing tests
if [ $TESTS_EXIT_CODE -ne 0 ] && [ "$FLIPBOOK_RUN" == 'False' ]; then
  die "Xamarin Integration tests failed"
fi
