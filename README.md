# nhsonline-web

> NHS Online Web

## Get code
Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-web
```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-web
```

## Installation
``` bash
# install dependencies
npm install
```

``` bash
# build for production with minification
npm run build
 
# build for production and view the bundle analyzer report
npm run build --report
```

``` bash
# compile client from swagger yaml
npm run swagger
```

## Running
``` bash
# compile swagger client and serve with hot reload at localhost:8080
npm run dev
```

``` bash
# run server with hot reload without generating swagger client
npm run dev-server
```

``` bash
# bring up docker
npm run docker-up
 
# take down docker
npm run docker-down
```

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
