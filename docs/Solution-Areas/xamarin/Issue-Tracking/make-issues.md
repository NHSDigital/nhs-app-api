# Xamarin

## Issue Tracking

### Mac- `Error response from daemon: pull access denied for local/nhsonline-integration-tests` - Problem

Running make run in the xamarinintegrationtests directory throws up this error on mac

```bash
docker: Error response from daemon: pull access denied for local/nhsonline-integration-tests, repository does not exist or may require 'docker login': denied: requested access to the resource is denied.
```

### Mac - Error response from daemon: pull access denied for local/nhsonline-integration-tests - Solution

Docker desktop > Preferences > Experimental features > Disable gRPC FUSE for file sharing

### Mac - `make: buildscripts/<bashscript> Permission denied` - Problem

Running make build in xamarinintegrationtests directory throws this error

```bash
make: buildscripts/<bashscript> Permission denied`
```

### Mac - `make: buildscripts/<bashscript> Permission denied` - Solution

```bash
chmod +x buildscripts/<bashscript>
```
