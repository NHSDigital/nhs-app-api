#!/bin/bash

DASHED_VERSION=$(cat /app/build/dashed_version);
/app/build/env.sh ./app/$DASHED_VERSION/config.json;