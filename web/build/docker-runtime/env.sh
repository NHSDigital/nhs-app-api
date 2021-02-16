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

  . "/app/build/env-vars.sh"

  echo "Completed Generating web config json"

  echo "${config}" >> "${outputFile}"
}

main 
