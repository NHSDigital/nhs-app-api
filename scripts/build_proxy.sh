#!/bin/bash

set -o nounset errexit pipefail

# Collect the API Proxy and Hosted Target
# files into build/apiproxy/ and deploy to Apigee

rm -rf build/proxies
mkdir -p build/proxies/live
cp -Rv proxies/live/apiproxy build/proxies/live