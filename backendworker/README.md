# Backend Worker

The backend worker is composed of the Router and IM1 Bridge components as described in the [Backend Logical Architecture](https://confluence.service.nhs.uk/display/NO/Target+Architecture+-+Backend+Logical).

## Running

Typically the backend workers are spun up in Docker containers using the root Makefile.

## Updating service journey rules

If you are updating service journey rule files, you need to delete the volumes to allow it to be recreated.

```bash
docker volume ls | grep servicejourneyrules | awk '{print $2}' | xargs docker volume rm
```

Furthermore, if changing the validation/loading logic, build service journey rules image.

```bash
make build
```

## Example curls to verify

### STUBS

curl -v \
  --header "Content-Type: application/json" \
  --header "X-API-ApplicationId: D66BA979-60D2-49AA-BE82-AEC06356E41F" \
  --header "X-API-Version: 2.1.0.0" \
  -d '' \
  stubs.local.bitraft.io:8800/emis/sessions/endusersession

### BACKEND WORKDER

curl -v \
  --header "Content-Type: application/json" \
  --header "NHSO-Connection-Token: token" \
  --header "NHSO-ODS-Code: E87649" \
  stubs.local.bitraft.io:8080/patient/im1connection

## Clinical Decision Support Setup

### Setup

#### Update hosts

Add the following to `/etc/hosts`

```
127.0.0.1       ems.cdss.stubs.local.bitraft.io
```

#### CDSS Supplier Stub

Currently the CDS Api depends on the CDSS Wiremock Stubs under `nhsonline-app/backendworker/NHSOnline.Backend.PfsApi/ClinicalDecisionSupport/cdss-wiremock`.
This is being used as a temporary CDSS supplier. Follow the instructions in the README within that project to get it running.
