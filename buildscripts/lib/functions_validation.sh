#! /usr/bin/env bash

function validate_config_file_present() {
  if [ ! -r "$1" ]; then
    die "'$1' is missing, please refer to [Getting Started](https://dev.azure.com/nhsapp/NHS App/_git/nhsapp?path=%2Fdocs%2Fgetting-started.md) to create this"
  fi
}

function validate_npm_settings () {
  validate_config_file_present "${NPMRC_PATH}"
}

function validate_maven_settings () {
  validate_config_file_present "${MVN_CFG_PATH}"
}
