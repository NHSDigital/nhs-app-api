#! /usr/bin/env bash
set -e

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1
mkdir -p reports

docker cp \
  nhsonline-web-test-run:/data/test/unit/coverage \
  reports/.

docker cp \
  nhsonline-web-test-run:/data/junit.xml \
  web-unit-test-results.xml

docker cp \
  nhsonline-web-test-run:/data/test/unit/coverage/cobertura-coverage.xml \
  web-unit-test-coverage.xml

# Fixup coverage source path
sed "s#\s*<source>.*</source>#<source>.</source>#" \
  web-unit-test-coverage.xml > web-unit-test-coverage.xml.tmp \
  && mv web-unit-test-coverage.xml.tmp web-unit-test-coverage.xml
