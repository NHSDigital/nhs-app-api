# Running `npm run dev` with non-default vars

As the [envBuilder](../functionality-documentation/env-builder.md) currently only scans the base `nhsapp/docker-compose.yml` file, you'll need to manually create the `env.json` when looking to run web using npm with any non-default options, e.g. `LOGINENV=sandpit`.

To get around this:
- Run whatever `make` command with `WEB=host` to bring up the services
- In terminal, run `docker ps | grep web` and copy the Container ID
- Then run `docker inspect --format='{{json .Config.Env}}' $CONTAINER_ID`
- Paste the output into `web/src/config/env.json` (create it if it doesn't exist) and reformat as a json object
- Remove `npm run build-env-json && ` from the `dev` script in `package.json`
- Run `npm run dev`
- Observe http://web.local.bitraft.io:3000/config.json to verify the configuration has been applied correctly

> Looking to get this built into the build-env-json script soon™