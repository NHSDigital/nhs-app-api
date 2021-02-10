#! /usr/bin/env bash
set -e

# Change current working directory to be the root of integration tests, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

DOCKER_ARGS=()
DOCKER_ARGS+=(--rm)
DOCKER_ARGS+=(-v "${REPO_ROOT}/xamarinintegrationtests:${DOCKER_ROOT}work")

pull_docker_image "${DOCKER_IMAGE_XSLTPROC}"

while IFS= read -r -d '' file
do
  REPORT_FILE=TestResults/$(basename "$file" .trx).md
  echo "Transforming $file => $REPORT_FILE"
  docker run \
    "${DOCKER_ARGS[@]}" \
    "${DOCKER_IMAGE_XSLTPROC}" trx.xslt "$file" > "$REPORT_FILE"

  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.addattachment type=Distributedtask.Core.Summary;name=Test Report;]$(realpath "$REPORT_FILE")"
  else
    cat "$REPORT_FILE"
  fi
done < <(find TestResults -name '*.trx' -print0)
