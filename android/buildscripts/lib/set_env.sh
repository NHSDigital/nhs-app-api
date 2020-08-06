#! /usr/bin/env bash

# shellcheck source=../../../buildscripts/lib/set_env.sh
source "../buildscripts/lib/set_env.sh"

GRADLE_PATH="${HOME}/.gradle"

if [[ $(uname -s) =~ ^MING.* ]]; then
  GRADLE_PATH="${USERPROFILE}/.gradle"
fi

DOCKER_ARGS=(--rm)

if [ -n "${TF_BUILD}" ]; then
  # cache gradle files when running in devops
  mkdir -p "${GRADLE_PATH}"
  DOCKER_ARGS+=(-v "${GRADLE_PATH}:${DOCKER_ROOT}data/.gradle")
else
  # cache gradle files locally in a volume
  if [ -n "$(docker volume ls -q -f "name=${GRADLE_CACHE_VOLUME}")" ]; then
    docker volume create "${GRADLE_CACHE_VOLUME}"
  fi

  DOCKER_ARGS+=(-v "${GRADLE_CACHE_VOLUME}:${DOCKER_ROOT}data/.gradle")
fi

DOCKER_ARGS+=(-v "${REPO_ROOT}:${DOCKER_ROOT}data/repo")
DOCKER_ARGS+=(-v "${MVN_CFG_PATH}:${DOCKER_ROOT}root/.m2/settings.xml")

DOCKER_ARGS+=(-w "${DOCKER_ROOT}data/repo/android")

DOCKER_ARGS+=(-e "GRADLE_USER_HOME=${DOCKER_ROOT}data/.gradle")

GRADLE_ARGS=("-Dorg.gradle.jvmargs='-XX:+UnlockExperimentalVMOptions -XX:+UseCGroupMemoryLimitForHeap'")

export DOCKER_ARGS
export GRADLE_ARGS
