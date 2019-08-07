#!/bin/bash

# Change current working directory to be the root of web, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1
mkdir -p reports

docker cp \
  nhsonline-web-test-run:/opt/app/test/unit/coverage \
  reports/.

docker cp \
  nhsonline-web-test-run:/opt/app/junit.xml \
  web-unit-test-results.xml

docker cp \
  nhsonline-web-test-run:/opt/app/test/unit/coverage/cobertura-coverage.xml \
  web-unit-test-coverage.xml

# Fixup coverage source path
echo Coverage file head
head -n 10 web-unit-test-coverage.xml
echo Running sed
sed -i '' -e "s/\s*<source>.*<\/source>/<source>.<\/source>/" web-unit-test-coverage.xml
echo Coverage file head
head -n 10 web-unit-test-coverage.xml
