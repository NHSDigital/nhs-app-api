#!/bin/bash
function test_connectivity() {
  local acr=$1            # The Azure Container Registry name
  local counter=1         # Count of loops that have been executed
  local connection=false  # Boolean to determine whether the connection has been successfully established
  local maxAttempts=10    # Maximum allowed loops while we wait for ACR firewall change to propagate
  local waitTime=5        # Max wait time (in seconds) between loops

  until [[ "${connection}" == true ]] || [[ "${counter}" == "${maxAttempts}" ]]; do
    sleep "${waitTime}"
    test=$(az acr check-health -n "${acr}" -y --ignore-errors 2>&1)
    if [[ "${test}" == *"CONNECTIVITY_REFRESH_TOKEN_ERROR"* ]]; then
      echo 'Error, please try again'
      ((counter=counter+1))
    else
      connection=true
    fi
  done

  if [[ "${counter}" == "${maxAttempts}" ]]; then
    echo 'Maximum attempts reached, failing...'
    exit 1
  fi
}

function add_acr_rule() {
  local acr=$1            # The Azure Container Registry name
  local mode=$2           # Mode for the script to run: add or remove
  local ip=$3             # IP address of the hosted agent
  local counter=1         # Count of loops that have been executed
  local outcome=false     # Boolean to determine whether the connection has been successfully established
  local maxAttempts=10    # Maximum allowed loops while we wait for ACR firewall change to propagate
  local waitTime=5        # Max wait time (in seconds) between loops

  until [[ "${outcome}" == true ]] || [[ "${counter}" == "${maxAttempts}" ]]; do
    echo "Attempt #${counter}"
    result=$(az acr network-rule "${mode}" -n "${acr}" --ip-address "${ip}" --query name -o tsv 2>&1 || true)
    if [[ "${result}" == "${acr}" ]]; then
      echo "IP address ${ip} successfully added to ACR ${acr}"
      outcome=true
    elif [[ "${result}" == *"duplicate"* ]]; then
      echo "IP Address ${ip} is already assigned."
      break
    elif [[ "${result}" == *"Another operation"* ]]; then
      echo "Concurrent operation in progress, trying again in ${waitTime} seconds..."
      sleep "${waitTime}"
      ((counter=counter+1))
    else
      echo "Attempt #${counter} unsuccessful, trying again in ${waitTime} seconds..."
      sleep "${waitTime}"
      ((counter=counter+1))
    fi
  done

  if [[ "${counter}" == "${maxAttempts}" ]]; then
    echo "Maximum attempts ${maxAttempts} reached, exiting..."
    exit 1
  fi
}

function main() {
  local acr=$1
  local mode=$2
  local ip

  ip=$(curl -s http://ipinfo.io/json | jq '.ip' | sed -e 's/^"//' -e 's/"$//')
  echo "agent host IP: ${ip}"
  # try to assign the ip
  add_acr_rule "${acr}" "${mode}" "${ip}"
  echo "IP address ${ip} successfully ${mode}ed -- ACR ${acr}"
  if [[ "${mode}" == 'add' ]]; then
    echo "Testing connectivity..."
    test_connectivity "${acr}"
  fi
}

main "$@"