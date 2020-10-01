# EnvBuilder

In short, this script
- scans the base **nhsapp / docker-compose.yml** file
- searches for the `web.local.bitraft.io` service
- when found, looks for the `env_file:` section
- if found, extracts the env file paths
- reads each env file into a json object
- outputs the result into **src / config / env.json**

This can be run at any time using `npm run build-env-json`, but runs by default as part of `npm run dev`.

Currently this is limited to generating the default configuration. If you want to run web locally with non-default configuration, see the [running npm run dev with non-default vars](/Solution-Areas/web/How-Tos/non-default-npm-run-dev) documentation for a work-around that works with any `make` command.