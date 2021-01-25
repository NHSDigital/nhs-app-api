# Adding a new environment variable

## Define environment variable

- Update/create a **.env** file in the **nhsapp/docker** directory with the new environment variable and a default value
  - If a new file, in the **nhsapp/docker-compose.yml** add it to the `web.local.bitraft.io` service

## Update `env.sh`

This script is used to build the **config.json** file that is served at `<WEB_URL>/config.json` (when built and ran using Docker; see **web/Dockerfile**).

- Open **web/build/docker-runtime/env.sh**
- Add the new environment variable with the appropriate envBuilder function name
  - e.g. AddBoolConfig NEW_ENV_VAR

---

When running web using `npm run dev`, the aforementioned **config.json** is served and built slightly differently, but currently relies on the same environment variables being defined in a **.env** file as per above.

For further information, see the [envBuilder documentation](/Solution-Areas/web/Functionality-Documentation/env-builder)