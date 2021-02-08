#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/set_env.sh
source "buildscripts/lib/set_env.sh"

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

mkdir -p reports

docker cp \
  nhsonline-web-test-run:/app/test/unit/coverage \
  reports/. || die "Failed to copy web coverage reports from container"

docker cp \
  nhsonline-web-test-run:/app/junit.xml \
  web-unit-test-results.xml || die "Failed to copy web unit test results from container"

docker cp \
  nhsonline-web-test-run:/app/test/unit/coverage/cobertura-coverage.xml \
  web-unit-test-coverage.xml || die "Failed to copy web coverage data from container"

# Fixup coverage source path
(sed "s#\s*<source>.*</source>#<source>.</source>#" \
  web-unit-test-coverage.xml > web-unit-test-coverage.xml.tmp \
  && mv web-unit-test-coverage.xml.tmp web-unit-test-coverage.xml) \
  || die "Failed to update source in coverage data"
