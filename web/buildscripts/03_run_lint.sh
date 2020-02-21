#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

if [ 1 -eq "$(docker ps -a | grep -c nhsonline-web-lint)" ]
then
  docker rm nhsonline-web-lint
fi

set +e

docker run \
  --rm \
  --name nhsonline-web-lint \
  local/nhsonline-web-build \
  npm run lint-raw

LINT_EXIT_CODE=$?

if [ -n "$TF_BUILD" ] && [ $LINT_EXIT_CODE -ne 0 ]; then
  echo "##vso[task.logissue type=error]Lint Failed"
  echo "##vso[task.complete result=Failed;]Lint Failed"
else
  exit $LINT_EXIT_CODE
fi
