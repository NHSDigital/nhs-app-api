# Adding a new environment variable

## **Define environment variable**

- Update/create a `.env` file in the [docker directory](../../../../docker) with the new environment variable and a default value
  - If a new file, in the [base docker-compose file](../../../../docker-compose.yml) add it to the `web.local.bitraft.io` service

## **Update `env.sh`**

This script is used to build the `config.json` file that is served at `<WEB_URL>/config.json` (when built and ran using Docker; see the Web [Dockerfile](../../../../web/Dockerfile)).

- Open `web/build/env.sh`
- Add a new key-value pair for the new variable
  - e.g. `"MY_NEW_ENV_VAR": "'"$MY_NEW_ENV_VAR"'"`

---

When running web using `npm run dev`, the aforementioned `config.json` is served and built slightly differently, but currently relies on the same environment variables being defined in a `.env` file as per above.

For further information, see the [envBuilder documentation](../functionality-documentation/env-builder.md)