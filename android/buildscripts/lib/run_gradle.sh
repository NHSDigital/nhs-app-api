#! /usr/bin/env bash

docker run \
  "${DOCKER_ARGS[@]}" \
  nhsapp.azurecr.io/nhsonline-android-build:jdk8-android_sdk_latest-1.0 \
  bash -c "\
    if [ -f 'local.properties' ]; then \
      rm -f 'local.properties'; \
    fi; \
    ./gradlew --no-daemon ${GRADLE_ARGS[*]}; \
    gradle_result=\$?; \
    chmod -R 755 /data/.gradle/daemon;
    exit \$gradle_result
  " || die "Android ${SCRIPT_NAME:-gradle} failed"
