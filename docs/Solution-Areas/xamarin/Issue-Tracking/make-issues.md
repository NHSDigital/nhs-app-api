# Xamarin

## Issue Tracking

#### Problem
Running make run in the xamarinintegrationtests directory throws up this error on mac
`docker: Error response from daemon: pull access denied for local/nhsonline-integration-tests, repository does not exist or may require 'docker login': denied: requested access to the resource is denied.`

#### Solution
Docker desktop > Preferences > Experimental features > Disable gRPC FUSE for file sharing

#### Problem 
Running make build in xamarinintegrationtests diretory throws this error
`make: buildscripts/<bashscript> Permission denied`

#### Solution
`chmod +x buildscripts/<bashscript>`