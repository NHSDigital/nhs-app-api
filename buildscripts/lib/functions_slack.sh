#! /usr/bin/env bash

function send_slack_message() {
  info "slack message body to send: $1"

  curl -X POST -H 'Content-type: application/json' \
    --data "$1" \
    "$2"
}

function build_xamarin_slack_message_body() {
  local upgrade_type=$1
  shift
  local package_upgrade_list=("$@")
  local package_upgrade_json_array=()

  info "BUILD_BUILDID: $BUILD_BUILDID"

  if [ ${#package_upgrade_list[@]} -eq 0 ]; then
    package_upgrade_json_array+=("{ \"type\": \"section\", \"text\": { \"type\": \"mrkdwn\", \"text\": \"No packages require manual upgrade of $upgrade_type versions. Stand down.\" } }")
  else
    for package_upgrade in "${package_upgrade_list[@]}"
    do
       package_upgrade_trim_trailing_newline=$(echo -e "$package_upgrade" | tr -d '\012\015')
       package_upgrade_json_array+=("{ \"type\": \"section\", \"text\": { \"type\": \"mrkdwn\", \"text\": \"$package_upgrade_trim_trailing_newline\" } }")
    done
  fi

  prefix_body_json="{
   \"blocks\": [
     {
       \"type\": \"section\",
       \"text\": {
         \"type\": \"mrkdwn\",
         \"text\": \"Weekly Nukeeper pipeline build ($BUILD_BUILDID) for Xamarin projects. Packages requiring manual upgrades of $upgrade_type versions are listed below.\"
       }
     },
     {
       \"type\": \"divider\"
     },"

  postfix_body_json="{
       \"type\": \"divider\"
     },
     {
       \"type\": \"actions\",
       \"elements\": [
         {
           \"type\": \"button\",
           \"text\": {
             \"type\": \"plain_text\",
             \"text\": \"Go to Xamarin Nukeeper pipeline run\",
             \"emoji\": true
           },
           \"value\": \"go_to_build\",
           \"url\": \"https://dev.azure.com/nhsapp/NHS%20App/_build/results?view=logs&buildId=$BUILD_BUILDID\"
         }
       ]
     },
     {
       \"type\": \"context\",
       \"elements\": [
         {
           \"type\": \"mrkdwn\",
           \"text\": \"Once we move to .NET6 Maui this manual step will be redundant and the Xamarin packages should be un-excluded from the Nukeeper script.\"
         }
       ]
     }
   ]
 }"

  echo "${prefix_body_json}$(join_by , "${package_upgrade_json_array[@]}"),${postfix_body_json}"
}
