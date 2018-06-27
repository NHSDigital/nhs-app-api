#!/bin/bash

certificate=~/.nhsonline/local-development-certificate/local-development-https.crt

if [ -f $certificate ]; then
  docker-compose -f docker-compose.yml -f docker-compose.https-android.yml up "$@"
else
  (>&2 echo ERROR: Could not find $certificate )
  (>&2 echo ERROR: Please run create-certificate.sh )
fi
