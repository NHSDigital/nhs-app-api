The backend worker is composed of the Router and IM1 Bridge components as described in the [Backend Logical Architecture](https://confluence.service.nhs.uk/display/NO/Target+Architecture+-+Backend+Logical).

## Running
Typically the backend worker is spun up in a Docker container from the web project.

### Adding TPP certificate locally
In order to use TPP integration in your local docker environment you need to add TPP certificate manually and set its password in environment variables.

The certificate and password can be found in keybase under path /team/nhsonline/gp practice details.

Steps to follow:
--
1. Copy /tpp_certificates/TppNhsTest.pfx certificate to /nhsonline-backendworker/NHSOnline.Backend.Worker/certs
2. Copy docker-compose.override.yml (sets TPP_CERTIFICATE_PASSWORD env variable) from keybase root folder into:
   - backendworker folder
   - web folder
   - bddtests folder

### Adding Vision certificate locally
In order to use Vision integration in your local docker environment, you need to add the Vision certificate manually and set its password in environment variables.

The certificate and password can be found in keybase under path /team/nhsonline/gp practice details.

Steps to follow:
--
1. Copy /vision_certs/vps_nhson001.pfx certificate to /nhsonline-backendworker/NHSOnline.Backend.Worker/certs
2. Copy docker-compose.override.yml (sets VISION_CERT_PASSPHRASE env variable) from keybase root folder into:
   - backendworker folder
   - web folder

### Enabling nominated pharmacy and copying the Spine LDAP certificate locally
In order to use the nominated pharmacy feature locally, you will need to enable two flags, copy a certificate manually and set its password in environment variables.

The certificate and password can be found in keybase under path /team/nhsonline/spine.

1. Copy /spine.pfx certificate to /nhsonline-backendworker/NHSOnline.Backend.Worker/certs
2. Copy docker-compose.override.yml (sets SPINE_CERT_PASSWORD env variable under pfs api) from keybase root folder into:
   - backendworker folder
   - web folder

We haven't been able to stub the response to the LDAP query in the BDD tests, therefore SPINE_LDAP_LOOKUP_ENABLED is false there, and instead some dummy values are used.

### Running the backend worker on its own
There are circumstances, however, when you will want to spin up the backend worker on its
own (for example when running the web locally using `npm run dev`).  To launch the worker, and the stubs run the following in the backend worker directory:

```
docker-compose up --build
```

NOTE: if buiding for the first time, build SJR service first:

```
docker-compose -f docker-compose.servicejourneyrules.yml build
```

#### Updating service journey rules
If you are updating service journey rule files, you need to delete the volumes to allow it to be recreated.

```
docker volume ls | grep servicejourneyrules | awk '{print $2}' | xargs docker volume rm
```

Furthermore, if changing the validation/loading logic, build service journey rules image prior to `docker-compose`.

```
docker-compose -f docker-compose.servicejourneyrules.yml build
```

### Debugging the backendworker
If you need to debug the backend worker and want to run it using Visual Studio, Visual Studio Code or Rider you will need to run Wiremock on its own.  To do this run the following in the backend worker directory:

```
docker-compose -f docker-compose.stubs.yml up
```

Secondly, you will need top copy any certificates you find inside the *gp practice details* directory in Keybase into the base of the *certs* directory inside *NHSOnline.Backend.Worker*.

Finally, you will also need to create a second appsettings file called *appsettings.Development.json.* Add any values you find in *docker-compose.override.yml* to this file as well. Whenever you start the debugger, the values inside *Properties/launchSettings.json* will be combined with the files *appsettings.Development.json* to form your configuration.

*appsettings.Development.json* is ignored by Git.

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

# Clinical Decision Support Setup

<br>
<br>

## Setup
--------

### Update hosts

Add the following to `/etc/hosts`

```
127.0.0.1       ems.cdss.stubs.local.bitraft.io
```

### CDSS Supplier Stub

Currently the CDS Api depends on the CDSS Wiremock Stubs under `nhsonline-app/backendworker/NHSOnline.Backend.PfsApi/ClinicalDecisionSupport/cdss-wiremock`.
This is being used as a temporary CDSS supplier. Follow the instructions in the README within that project to get it running.
