#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

if [ 1 -eq "$(docker ps -a | grep -c nhsonline-web-test-run)" ]
then
  docker rm nhsonline-web-test-run
fi

docker run \
  --name nhsonline-web-test-run \
  "${ARGS[@]}" \
  local/nhsonline-web-build \
  npm run test-jest || die "Web unit tests failed"
