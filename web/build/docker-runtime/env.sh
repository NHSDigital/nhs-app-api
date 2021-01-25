#!/bin/bash

AddStringConfig() {
  local env_var=$1
  if [ -z "$env_var" ] ; then
    echo "AddStringConfig: no function argument"; exit 1
  fi

  local value=${!env_var}
  if [ -z "$value" ] ; then
    echo "AddStringConfig: $env_var has a null value"; exit 1
  fi

  config=$(echo "$config" | jq --arg key $env_var --arg val $value '. + {($key): $val}')
}

AddBoolConfig() {
  local env_var=$1
  if [ -z "$env_var" ] ; then
    echo "AddBoolConfig: no function argument"; exit 1
  fi

  local value=${!env_var}
  if [ -z "$value" ] ; then
    echo "AddBoolConfig: $env_var has a null value"; exit 1
  fi

  if ! [[ $value = true || $value = false ]] ; then
    echo "error: $env_var=$value is not a boolean, but is being configured to be one" >&2; exit 1
  fi

  config=$(echo "$config" | jq --arg key $env_var --arg val $value '. + {($key): $val | test("true")}')
}

AddNumericConfig() {
  re='^[+-]?[0-9]+([.][0-9]+)?$'
  local env_var=$1
  if [ -z "$env_var" ] ; then
    echo "AddNumericConfig: no function argument"; exit 1
  fi

  local value=${!env_var}
  if [ -z "$value" ] ; then
    echo "AddNumericConfig: $env_var has a null value"; exit 1
  fi

  if ! [[ $value =~ $re ]] ; then
    echo "error: $env_var=$value is not a number, but is being configured to be one" >&2; exit 1
  fi
  config=$(echo "$config" | jq --arg key $env_var --arg val $value '. + {($key): $val | tonumber}')
}

config='{}';
echo "Begin Generating web config json"
AddNumericConfig PORT;
AddBoolConfig THIRD_PARTY_JUMP_OFF_LOGGING_ENABLED;
AddStringConfig URI_FORMAT_API_CLIENT;
AddStringConfig URI_FORMAT_CID_REDIRECT_WEB;
AddStringConfig URI_FORMAT_CID_REDIRECT_NATIVE;
AddBoolConfig SECURE_COOKIES;
AddStringConfig CID_CLIENT_ID;
AddStringConfig CID_AUTH_ENDPOINT_URL;
AddBoolConfig CID_P5_VECTOR_OF_TRUST_ENABLED;
AddStringConfig CID_SETTINGS_URL;
AddStringConfig BLOOD_DONATION_URL;
AddStringConfig DATA_PREFERENCES_URL;
AddStringConfig ANALYTICS_SCRIPT_URL;
AddStringConfig ANALYTICS_ENVIRONMENT;
AddStringConfig HOTJAR_SITE_ID;
AddStringConfig HOTJAR_SURVEY_URL;
AddBoolConfig HOTJAR_SURVEY_VISIBLE;
AddStringConfig CONTACT_US_URL;
AddBoolConfig USER_RESEARCH_ENABLED;
AddStringConfig VERSION_TAG;
AddStringConfig COMMIT_ID;
AddBoolConfig CE_MARK_ENABLED;
#AddBoolConfig ORGAN_DONATION_INTEGRATION_ENABLED;
AddNumericConfig SESSION_EXPIRING_WARNING_SECONDS;
AddBoolConfig SIXTEEN_WEEKS_SLOTS_ENABLED;
AddBoolConfig ADD_APPOINTMENT_TO_CALENDAR_ENABLED;
AddStringConfig CORONA_SERVICE_URL;
AddStringConfig CONDITIONS_CHECKER_URL;
AddStringConfig SYMPTOM_CHECKER_URL;
AddStringConfig EMERGENCY_PRESCRIPTIONS_URL;
AddBoolConfig VUE_WINDOW_OBJECT_ENABLED;
echo "Completed Generating web config json"

echo $config >> "${1}"
