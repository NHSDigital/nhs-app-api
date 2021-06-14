#! /usr/bin/env bash
set -e
shopt -s expand_aliases

# Change current working directory to be the root of backendworker, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

alias recursiveSearch='grep --recursive --only-matching --extended-regexp --no-filename'
alias nonRecursiveSearch='grep --only-matching --extended-regexp --no-filename'

function validate_chart_files() {
  recursiveSearch 'servicesV3__.(.*http.*)' ../nhsapp-chart/vars | nonRecursiveSearch 'http.*' | sort - > servicesv3chart.tmp
  
  recursiveSearch 'services__.(.*http.*)' ../nhsapp-chart/vars | nonRecursiveSearch 'http.*' | sort - > serviceslegacychart.tmp

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
  recursiveSearch 'servicesV3__.(.*http.*)' ../docker/*.env | nonRecursiveSearch 'http.*' | sort -> servicesv3docker.tmp
  
  recursiveSearch 'services__.(.*http.*)' ../docker/*.env | nonRecursiveSearch 'http.*' | sort -> serviceslegacydocker.tmp

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
  validate_chart_files
  validate_docker_env_files
}

main "$@"