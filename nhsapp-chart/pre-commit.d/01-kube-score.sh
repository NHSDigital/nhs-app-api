#!/bin/sh
#
# Pre-commit script to run kube-score static code analysis 
# https://github.com/zegl/kube-score

set -e

helm template ../nhsonline/ | kube-score score - \
--ignore-test container-image-pull-policy \
--ignore-test container-image-tag
