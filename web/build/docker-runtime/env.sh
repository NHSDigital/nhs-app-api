#! /usr/bin/env bash
set -e

NUMBER_REGEX="^[+-]?[0-9]+([.][0-9]+)?$"

outputFile="$1"
config="{}"

errorAndExit() {
  local errorMessage="$1"

  echo "ERROR ${errorMessage}" >&2
  exit 1
}

getEnvVarValue() {
  local envVar="$1"
  local functionName="${FUNCNAME[1]}"

  if [ -z "$envVar" ] ; then
    errorAndExit "${functionName}: no function argument"
  fi

  local value="${!envVar}"

  if [ -z "$value" ] ; then
    errorAndExit "${functionName}: $envVar has a null value"
  fi

  echo "${value}"
}

addStringConfig() {
  local envVar="$1"
  local value="$(getEnvVarValue "${envVar}")"

  config=$(echo "$config" | jq --arg key $envVar --arg val $value '. + {($key): $val}')
}

addBoolConfig() {
  local envVar="$1"
  local value="$(getEnvVarValue "${envVar}")"

  if ! [[ $value = true || $value = false ]] ; then
    errorAndExit "$envVar=$value is not a boolean, but is being configured to be one"
  fi

  config=$(echo "$config" | jq --arg key $envVar --arg val $value '. + {($key): $val | test("true")}')
}

addNumericConfig() {
  local envVar="$1"
  local value="$(getEnvVarValue "${envVar}")"

  if ! [[ $value =~ $NUMBER_REGEX ]] ; then
    errorAndExit "$envVar=$value is not a number, but is being configured to be one"
  fi

  config=$(echo "$config" | jq --arg key $envVar --arg val $value '. + {($key): $val | tonumber}')
}

main () {
  echo "Begin Generating web config json"
  
  addNumericConfig "PORT"
  addBoolConfig "THIRD_PARTY_JUMP_OFF_LOGGING_ENABLED"
  addStringConfig "URI_FORMAT_API_CLIENT"
  addStringConfig "URI_FORMAT_CID_REDIRECT_WEB"
  addStringConfig "URI_FORMAT_CID_REDIRECT_NATIVE"
  addBoolConfig "SECURE_COOKIES"
  addStringConfig "CID_CLIENT_ID"
  addStringConfig "CID_AUTH_ENDPOINT_URL"
  addBoolConfig "CID_P5_VECTOR_OF_TRUST_ENABLED"
  addStringConfig "CID_SETTINGS_URL"
  addStringConfig "BLOOD_DONATION_URL"
  addStringConfig "DATA_PREFERENCES_URL"
  addStringConfig "ANALYTICS_SCRIPT_URL"
  addStringConfig "ANALYTICS_ENVIRONMENT"
  addStringConfig "HOTJAR_SITE_ID"
  addStringConfig "HOTJAR_SURVEY_URL"
  addBoolConfig "HOTJAR_SURVEY_VISIBLE"
  addStringConfig "CONTACT_US_URL"
  addBoolConfig "USER_RESEARCH_ENABLED"
  addStringConfig "VERSION_TAG"
  addStringConfig "COMMIT_ID"
  addBoolConfig "CE_MARK_ENABLED"
  #addBoolConfig "ORGAN_DONATION_INTEGRATION_ENABLED"
  addNumericConfig "SESSION_EXPIRING_WARNING_SECONDS"
  addBoolConfig "SIXTEEN_WEEKS_SLOTS_ENABLED"
  addBoolConfig "ADD_APPOINTMENT_TO_CALENDAR_ENABLED"
  addStringConfig "CORONA_SERVICE_URL"
  addStringConfig "CONDITIONS_CHECKER_URL"
  addStringConfig "SYMPTOM_CHECKER_URL"
  addStringConfig "EMERGENCY_PRESCRIPTIONS_URL"
  addBoolConfig "VUE_WINDOW_OBJECT_ENABLED"

  echo "Completed Generating web config json"

  echo "${config}" >> "${outputFile}"
}

main 
