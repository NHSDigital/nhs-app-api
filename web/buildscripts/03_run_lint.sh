#!/bin/bash

set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

if [ 1 -eq "$(docker ps -a | grep -c nhsonline-web-lint)" ]
then
  docker rm nhsonline-web-lint
fi

docker run \
  --name nhsonline-web-lint \
  local/nhsonline-web-build \
  npm run lint-raw
