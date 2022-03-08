#! /usr/bin/env bash
set -e

cd "$HOME/Library/Developer/Xamarin/android-sdk-macosx/emulator"

if [ -z "$EMULATOR" ]; then
    echo "No emulator specified using first in -list-avds"
    ./emulator -avd $(./emulator -list-avds | awk  'FNR == 1 {print}') -dns-server 127.0.0.1
else
    echo "Using emulator $EMULATOR"
    ./emulator -avd $EMULATOR -dns-server 127.0.0.1
fi

