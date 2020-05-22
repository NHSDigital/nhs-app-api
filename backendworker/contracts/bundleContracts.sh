#! /usr/bin/env bash
set -e

# Change current working directory to be the root of backendworker contracts, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")" || exit 1

npm run bundleContracts
