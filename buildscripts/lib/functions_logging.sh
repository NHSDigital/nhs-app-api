#! /usr/bin/env bash

# Note:
# This file is included from multiple buildscripts directories.
# No assumptions should be made about the current directory.

function die () {
  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.logissue type=error]$*"
    echo '##vso[task.complete result=Failed;]DONE'
    exit
  fi
  echo >&2 "===]> Error: $*"
  exit 1
}

function error () {
  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.logissue type=error]$*"
  else
    echo >&2 "===]> Error: $*"
  fi
}

function warn () {
  if [ -n "$TF_BUILD" ]; then
    echo "##vso[task.logissue type=warning]$*"
  else
    echo >&2 "===]> Warning: $*"
  fi
}

function info () {
  echo >&2 "===]> Info: $*";
}
