#! /usr/bin/env bash
set -x

docker run \
  "${DOCKER_ARGS[@]}" \
  "${DOCKER_IMAGE_ANDROID_BUILD}" \
  bash -c "\
    if [ -f 'local.properties' ]; then \
      rm -f 'local.properties'; \
    fi; \
    ./gradlew --no-daemon ${GRADLE_ARGS[*]}; \
    gradle_result=\$?; \
    chmod -R 755 /data/.gradle/daemon;
    exit \$gradle_result
  " || die "Android ${SCRIPT_NAME:-gradle} failed"
