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
DOCKER_ARGS+=(--env "Chrome__Arguments__headless=true")
DOCKER_ARGS+=(-v "//var/run/docker.sock:/var/run/docker.sock")

setup_browserstack_environment

cleanup_docker_containers

set_docker_compose_files_args

pull_docker_images

start_services_under_test

set +e

docker run \
  "${DOCKER_ARGS[@]}" \
  "${DOCKER_REGISTRY:-local}/nhsonline-integration-tests:${DOCKER_TAG:-latest}" bash -c '
    dotnet test --no-build -c Release -r /src/TestResults --logger trx NHSOnline.IntegrationTests/NHSOnline.IntegrationTests.csproj;
    TESTS_EXIT_CODE=$? ;
    rm -Rf /src/TestResults/Deploy* ;
    exit $TESTS_EXIT_CODE'

TESTS_EXIT_CODE=$?

set -e

docker cp int_test_test_runner:/src/TestResults/. TestResults

stop_services_under_test

fetch_container_logs

destroy_services_under_test

if [ $TESTS_EXIT_CODE -ne 0 ]; then
  if [ -n "$TF_BUILD" ]; then
    echo "##vso[build.addbuildtag]xamarin-tests-failed"
  else
    die "Xamarin Integration tests failed"
  fi
fi
