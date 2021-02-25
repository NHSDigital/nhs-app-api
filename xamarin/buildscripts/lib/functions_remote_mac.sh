#! /usr/bin/env bash

function configure_remote_mac () {
  if [ -x "remote_mac_config.sh" ]; then
    source "remote_mac_config.sh"
  elif [[ $(uname -s) =~ ^MING.* ]]; then
    info 'To build on Windows using a mac build server create remote_mac_config.sh containing:
#! /usr/bin/env bash
MSBUILD_ARGS+=("-p:ServerUser=[macusername]")
MSBUILD_ARGS+=("-p:ServerAddress=[machostname]")'
    exit
  fi
}
