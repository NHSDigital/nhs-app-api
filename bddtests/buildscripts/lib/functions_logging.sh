#!/bin/bash

function die () {
    echo >&2 "===]> Error: $* "
    exit 1
}

function info () {
    echo >&2 "===]> Info: $* ";
}
