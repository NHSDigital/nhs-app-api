#! /usr/bin/env bash

MVN_CONFIG_PATH="${HOME}/.m2/settings.xml"
GRADLE_PATH="${HOME}/.gradle"

DOCKER_ARGS=(--rm)
DOCKER_ROOT="/"
REPO_ROOT=$(cd ..; pwd)

if [[ $(uname -s) =~ ^MING.* ]]; then
  MVN_CONFIG_PATH="${USERPROFILE}/.m2/settings.xml"
  GRADLE_PATH="${USERPROFILE}/.gradle"
  DOCKER_ROOT="//"
  REPO_ROOT=$(cd ..; pwd -W)
fi

if [ -n "${TF_BUILD}" ]; then
  # cache gradle files when running in devops
  mkdir -p "${GRADLE_PATH}"
  DOCKER_ARGS+=(-v "${GRADLE_PATH}:${DOCKER_ROOT}data/.gradle")
fi

DOCKER_ARGS+=(-v "${REPO_ROOT}:${DOCKER_ROOT}data/repo")
DOCKER_ARGS+=(-v "${MVN_CONFIG_PATH}:${DOCKER_ROOT}root/.m2/settings.xml")

DOCKER_ARGS+=(-w "${DOCKER_ROOT}data/repo/android")

DOCKER_ARGS+=(-e "GRADLE_USER_HOME=${DOCKER_ROOT}data/.gradle")

GRADLE_ARGS=("-Dorg.gradle.jvmargs='-XX:+UnlockExperimentalVMOptions -XX:+UseCGroupMemoryLimitForHeap'")

export DOCKER_ARGS
export GRADLE_ARGS
