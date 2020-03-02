#! /usr/bin/env bash

docker run \
  "${DOCKER_ARGS[@]}" \
  nhsapp.azurecr.io/nhsonline-android-build:jdk8-android_sdk_latest-1.0 \
  bash -c "\
    if [ -f 'local.properties' ]; then \
      rm -f 'local.properties'; \
    fi; \
    ./gradlew --no-daemon ${GRADLE_ARGS[*]}; \
    test_run_result=\$?; \
    chmod -R 777 /data;
    exit \$test_run_result
  "
