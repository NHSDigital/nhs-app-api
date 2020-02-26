#!/bin/bash

# Run helm lint on nhsonline chart
#
# `helm lint` This command takes a path to a chart and runs a series of tests to verify that
# the chart is well-formed.
#
# If the linter encounters things that will cause the chart to fail installation,
# it will emit [ERROR] messages. If it encounters issues that break with convention
# or recommendation, it will emit [WARNING] messages.
#
# Usage: helm lint [flags] PATH
#
# This can be updated to fail on warnings by adding the `--strict` flag

set -e

helm lint --debug ./nhsonline