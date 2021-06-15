#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

create_certificate "$HOME/.nhsonline/secure-stubs-certificate"  '*.securestubs.local.bitraft.io' "secure-stubs-https"

