# nhsonline-app

> NHS Online App

## Get code
Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-app.git

```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-app.git
```

2. Copy docker-compose.override.yml (sets VISION_CERT_PASSPHRASE env variable) from keybase root folder into:
  - backendworker folder
  - web folder
  - bddtests folder

## Pipeline Tests
**NOTE:**
* Docker 17.05 or higher on the daemon and client is required (for example [docker4mac](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac)).
* Delete existing node_module folder if you already run `npm install` locally to prevent issues starting container with incompatible libraries

To run the pipeline tests in a fully containerised environment execute the build and test script

```bash
./build_and_test.sh
```

It can optionally be called with reference to a specific test tag e.g.

```bash
./build_and_test.sh @prescription
```

for full usage information for the script launch with the -h switch

```bash
./build_and_test.sh -h
```