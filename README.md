# nhsonline-web

> NHS Online Web

## Get code
Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-web
```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-web
```

## Run
**NOTE:**
* Docker 17.05 or higher on the daemon and client is required (for example [docker4mac](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac)).
* Delete existing node_module folder if you already run `npm install` locally to prevent issues starting container with incompatible libraries

```
docker-compose up --build
# open browser to http://localhost:3000
```

If for any reason you need to shell into container, use:

`docker exec -it $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1) /bin/sh`

Restart:

`docker kill $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1)`

Stop:

`docker kill $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1)`

**NOTE: If you need to regenerate SASS files, plese restart (or stop and run again) container.**

## Tests
``` bash
# run unit tests
npm run test-unit

# run e2e (browser) tests including taking down and bringing up the docker containers.
npm run test-e2e

# e2e in headless mode
npm run test-e2e -- --headless

# e2e with a different base URL
npm run tests-e2e -- --base_url http://alternative.base.url/

# run browser tests but do not attempt to bring up or take down docker (assume it to be available).
npm run test-browser

# browser in headless mode
npm run test-browser -- --headless

# browser with a different base URL
npm run test-browser -- --base_url http://alternative.base.url/

# run all tests
npm test
```

For a detailed explanation on how things work, check out the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).
