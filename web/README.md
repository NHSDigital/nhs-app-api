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

## Other build options

To get a list of available build options run make

```bash
$ make
build                          Build all web components
test                           Unit test all web components
help                           This help.
```

**NOTE: If you need to regenerate SASS files, please restart (or stop and run again) the container.**

## Tests

``` bash
# run unit tests
npm run test

# run all tests
npm test
```

For a detailed explanation on how things work, check out the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).
