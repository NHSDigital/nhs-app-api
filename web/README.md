# NHS Online Web

## Setup

Before running the web locally, some entries need to be added to your machine's `hosts` file (`/etc/hosts` on Mac or `C:\windows\system32\drivers\etc\hosts` on Windows) to add the following entries:

```bash
127.0.0.1       web.local.bitraft.io
127.0.0.1       api.local.bitraft.io
127.0.0.1       stubs.local.bitraft.io
127.0.0.1       cid.local.bitraft.io
127.0.0.1       users.local.bitraft.io
127.0.0.1       servicejourneyrulesapi.local.bitraft.io
```

## Run

**NOTE:**

* Docker 17.05 or higher on the daemon and client is required (for example [docker4mac](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac)).
* Delete existing node_module folder if you already run `npm install` locally to prevent issues starting container with incompatible libraries

```bash
make run
# if using docker toolbox on Windows execute docker-port-forward.sh from /utils
# open browser to http://web.local.bitraft.io:3000
```

## Running with HTTPS

```bash
./create-certificate.sh    # follow the instructions to install the generated certificate
make run-https
# open browser to https://local.bitraft.io
```

If for any reason you need to shell into container, use:

`docker exec -it $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1) /bin/sh`

Restart:

`docker kill $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1)`

Stop:

`docker kill $(docker ps|grep local/dft-street-manager-ux|cut -d" " -f1)`

**NOTE: If you need to regenerate SASS files, plese restart (or stop and run again) container.**

## Other build/run options

To get a list of available build and run option run make

```bash
$ make
build                          Build all web components
test                           Unit test all web components
run                            Run in docker
run-biometrics                 Run in docker with biometrics enabled
run-https                      Run in docker with https enabled
run-android                    Run in docker for android
run-android-biometrics         Run in docker for android with biometrics enabled
help                           This help.
```

## Tests

``` bash
# run unit tests
npm run test

# run all tests
npm test
```

For a detailed explanation on how things work, check out the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).
