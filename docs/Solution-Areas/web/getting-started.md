# NHS Online Web

## Setup

[Host file entries](/Getting-Started#host-file-entries)

## Develop in Docker

### Init

```bash
make dev-init
```

### Run

Start other components

```bash
make -C .. run WEB=host
```

Start development server

```bash
make dev-run
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

## Running NPM Audit

We point to an NPM registry proxy in DevOps, because of this running the NPM audit command is a little different.

```bash
npm run audit
```

*Note: try to avoid using the `--fix` option as you will need to manually change `https://registry.npmjs.org` URLs in the `package-lock.json` file to `https://pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/registry` after running it*