# nhsonline-web

> NHS Online Web

## Get code
Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-web
```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-web
```

2. Copy docker-compose.override.yml (sets VISION_CERT_PASSPHRASE env variable) from keybase root folder into:
  - backendworker folder
  - web folder
  
## Setup
Before running the web locally, some entries need to be added to your machine's `hosts` file (`/etc/hosts` on Mac or `C:\windows\system32\drivers\etc\hosts` on Windows) to add the following entries:

```
127.0.0.1       web.local.bitraft.io
127.0.0.1       api.local.bitraft.io
127.0.0.1       stubs.local.bitraft.io
127.0.0.1       cid.local.bitraft.io
127.0.0.1       servicejourneyrulesapi.local.bitraft.io
```

## Run
**NOTE:**
* Docker 17.05 or higher on the daemon and client is required (for example [docker4mac](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac)).
* Delete existing node_module folder if you already run `npm install` locally to prevent issues starting container with incompatible libraries

```
docker-compose up --build
# if on windows execute docker-port-forward.sh from the nhsonline-dev-utils repository
# open browser to http://web.local.bitraft.io:3000
```

## Running with HTTPS
```
# in windows be sure to run this from the Docker Toolbox terminal
./create-certificate.sh    # follow the instructions to install the generated certificate
./run-with-https.sh
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
npm run test

# run all tests
npm test
```

For a detailed explanation on how things work, check out the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).

## Biometrics
To enable biometrics on android or iOS, the BIOMETRICS_ENABLED environment variable should be set to true.
To do this you can run
`docker-compose -f docker-compose.yml -f docker-compose.biometrics.yml up`
