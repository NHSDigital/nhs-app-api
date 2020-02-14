#!/bin/bash

function validate_maven_settings () {
  [ -f ~/.m2/settings.xml ] || die "~/.m2/settings.xml does not exist, please create this and add credentials to it according to the README"
}
