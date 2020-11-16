#!/bin/bash

envsubst '${WELL_KNOWN_ANDROID_PACKAGE_NAME},${WELL_KNOWN_ANDROID_FINGERPRINT}' < /app/.well-known/assetlinks.json.template > /app/.well-known/assetlinks.json;
rm /app/.well-known/assetlinks.json.template;
echo "Completed defining Universal Links";

envsubst '${WELL_KNOWN_IOS_APP_ID}' < /app/.well-known/apple-app-site-association.template > /app/.well-known/apple-app-site-association;
rm /app/.well-known/apple-app-site-association.template; 
echo "Completed defining App Links";