# Running Against Local Docker Env

A different command is required to get the `envBuilder.js` script to create a matching `src/config/env.json` when running web from npm and the local env from docker; this allows the use of any non-default options, e.g. `LOGINENV=sandpit`.

Ensure however you are running the local environment (run, bdd env, browserstack etc.) that you include `WEB=host` in the make command line to signal that you will run the web service manually yourself.

Doing this will start a dummy web container which proxies requests from inside the private docker network to your local web instance.

To start the local web service, simply open a terminal in the web directory and run:

```bash
npm install && npm run docker-dev
```

The dummy web container you started in docker will be read to get all variables. This will include any custom config for NHS Login environments etc.
