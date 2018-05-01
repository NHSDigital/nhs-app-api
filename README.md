The backend worker is composed of the Router and IM1 Bridge components as described in the [Backend Logical Architecture](https://confluence.service.nhs.uk/display/NO/Target+Architecture+-+Backend+Logical).

## Running
Typically the backend worker is spun up in a Docker container from the web project.

### Running the backend worker on its own
There are circumstances, however, when you will want to spin up the backend worker on its
own (for example when running the web locally using `npm run dev`).  To launch the worker, the stubs
and the Redis instances run the following in the backend worker directory:

```
docker-compose up --build
```

### Running Wiremock & Redis on their own
If you need to debug the backend worker and want to run it using Visual Studio or Rider you will
need to run Wiremock and Redis on their own.  To do this run the following in the backend
worker directory:

```
docker-compose up -f docker-compose.stubs.yml
```

## Example curls to verify
### STUBS
curl -v \
  --header "Content-Type: application/json" \
  --header "X-API-ApplicationId: D66BA979-60D2-49AA-BE82-AEC06356E41F" \
  --header "X-API-Version: 2.1.0.0" \
  -d '' \
  localhost:8800/emis/sessions/endusersession

### BACKEND WORKDER
curl -v \
  --header "Content-Type: application/json" \
  --header "NHSO-Connection-Token: token" \
  --header "NHSO-ODS-Code: E87649" \
  localhost:8080/patient/im1connection
