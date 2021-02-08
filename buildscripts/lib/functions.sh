#! /usr/bin/env bash

LIB_DIR=$(dirname "${BASH_SOURCE[0]}")

# shellcheck source=functions_logging.sh
source "${LIB_DIR}/functions_logging.sh"

# shellcheck source=functions_validation.sh
source "${LIB_DIR}/functions_validation.sh"

# shellcheck source=functions_docker.sh
source "${LIB_DIR}/functions_docker.sh"
