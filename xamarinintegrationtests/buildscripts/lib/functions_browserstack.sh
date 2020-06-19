#! /usr/bin/env bash

function validate_browserstack_environment () {
  if [ -z "$BrowserStack__Key" ] && [ -f ~/.nhsonline/secrets/browserstack_accesskey ]; then
    BrowserStack__Key=$(<~/.nhsonline/secrets/browserstack_accesskey)
    export BrowserStack__Key
  fi

  [ -n "$BrowserStack__Key" ] || die "BrowserStack__Key is not specified, this is required to run native integration tests"

  export BrowserStack__User=${BrowserStack__User:-ops20}

  DOCKER_ARGS+=(--env "BrowserStack__Key=${BrowserStack__Key}")
  DOCKER_ARGS+=(--env "BrowserStack__User=${BrowserStack__User:-ops20}")
  DOCKER_ARGS+=(--env "BrowserStack__Build=${BrowserStack__Build:-${HOSTNAME}-docker}")
}

function generate_browserstack_local_identifier () {
  local RANDOM_NUMBER

  # Generate a unique identifier that's predicable if we are running the tests locally (outside of docker)
  if [ "$RUN_LOCAL" == 1 ]; then
    BrowserStack__LocalIdentifier=${BROWSERSTACK_LOCAL_IDENTIFIER:-"${DOCKER_PROJECT_NAME}_${HOSTNAME}"}
  else
    RANDOM_NUMBER=$(printf "%4X%4X" $RANDOM $RANDOM | tr ' ' '0')
    BrowserStack__LocalIdentifier=${BROWSERSTACK_LOCAL_IDENTIFIER:-"${DOCKER_PROJECT_NAME}_${HOSTNAME}_${RANDOM_NUMBER}"}
  fi

  export BrowserStack__LocalIdentifier
  DOCKER_ARGS+=(--env "BrowserStack__LocalIdentifier=${BrowserStack__LocalIdentifier}")

  info "BrowserStack__LocalIdentifier=${BrowserStack__LocalIdentifier}"
}

function upload_android_app_to_browserstack () {
  local BROWSERSTACK_UPLOAD_RESPONSE

  [ -f "$NATIVE_APP_PATH_ANDROID" ] || die "Native app (NATIVE_APP_PATH_ANDROID=$NATIVE_APP_PATH_ANDROID) does not exist, this is required to run integration tests"

  info "Uploading native app to BrowserStack: $NATIVE_APP_PATH_ANDROID"

  local BROWSERSTACK_CUSTOM_ID=${BROWSERSTACK_CUSTOM_ID:-${HOSTNAME}-android}
  BROWSERSTACK_UPLOAD_RESPONSE=$(curl \
    -u "$BrowserStack__User:$BrowserStack__Key" \
    -X POST \
    -F "file=@$NATIVE_APP_PATH_ANDROID" \ \
    -F "data={\"custom_id\": \"$BROWSERSTACK_CUSTOM_ID\"}" \
    "https://api-cloud.browserstack.com/app-automate/upload")
  info "BROWSERSTACK_UPLOAD_RESPONSE: $BROWSERSTACK_UPLOAD_RESPONSE"

  # shellcheck disable=SC2001 #See if you can use ${variable//search/replace} instead.
  Android__App=$(echo "$BROWSERSTACK_UPLOAD_RESPONSE" | sed -e 's/^.*"app_url":"\([^"]*\)".*$/\1/')

  export Android__App
  DOCKER_ARGS+=(--env "Android__App=${Android__App}")

  info "Android__App: $Android__App"
}

function setup_browserstack_environment () {
  validate_browserstack_environment
  generate_browserstack_local_identifier
  upload_android_app_to_browserstack
}
