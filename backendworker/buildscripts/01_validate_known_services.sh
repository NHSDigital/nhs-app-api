#! /usr/bin/env bash
set -e
shopt -s expand_aliases

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

alias recursiveSearch='grep --recursive --only-matching --extended-regexp --no-filename'
alias nonRecursiveSearch='grep --only-matching --extended-regexp --no-filename'
alias invertMatchSearch='grep --invert-match'

function validate_json_files() {
  nonRecursiveSearch 'http(s)?://.*' NHSOnline.Backend.PfsApi/KnownServicesV3.json | cut -d '"' -f 1 | invertMatchSearch 'https://WebAppBaseUrl' | sort - > servicesv3json.tmp
  
  nonRecursiveSearch 'http(s)?://.*' NHSOnline.Backend.PfsApi/KnownServices.json | cut -d '"' -f 1 | invertMatchSearch 'https://WebAppBaseUrl' | sort - > serviceslegacyjson.tmp

  if diff serviceslegacyjson.tmp servicesv3json.tmp; then
    echo "Legacy and v3 Known Services JSON files match"
  else
    echo "The legacy and v3 Known Services JSON files are different. See above about for details"
    echo "< = url is present in Legacy known services, but missing in V3 known services"
    echo "> = url is present in V3 known services, but missing in Legacy known services"
    exit 1
  fi
}

function validate_chart_files() {
  recursiveSearch 'servicesV3__.(.*http(s)?://.*)' ../nhsapp-chart/vars | nonRecursiveSearch 'http(s)?://.*' | sort - > servicesv3chart.tmp
  
  recursiveSearch 'services__.(.*http(s)?://.*)' ../nhsapp-chart/vars | nonRecursiveSearch 'http(s)?://.*' | sort - > serviceslegacychart.tmp

  if diff serviceslegacychart.tmp servicesv3chart.tmp; then
    echo "Legacy and v3 Known Services chart files match"
  else
    echo "The legacy and v3 Known Services chart files are different. See above about for details"
    echo "< = url is present in Legacy known services, but missing in V3 known services"
    echo "> = url is present in V3 known services, but missing in Legacy known services"
    exit 1
  fi
}

function validate_docker_env_files() {
  recursiveSearch 'servicesV3__.(.*http(s)?://.*)' ../docker/*.env | nonRecursiveSearch 'http(s)?://.*' | sort - > servicesv3docker.tmp
  
  recursiveSearch 'services__.(.*http(s)?://.*)' ../docker/*.env | nonRecursiveSearch 'http(s)?://.*' | sort - > serviceslegacydocker.tmp

  if diff serviceslegacydocker.tmp servicesv3docker.tmp; then
    echo "Legacy and v3 Known Services docker env match"
  else
    echo "The legacy and v3 Known Services docker env files are different. See above about for details"
    echo "< = url is present in Legacy known services, but missing in V3 known services"
    echo "> = url is present in V3 known services, but missing in Legacy known services"
    exit 1
  fi
}

function main() {
  echo "Validating known services"
  validate_json_files
  validate_chart_files
  validate_docker_env_files
}

main "$@"