# NHS App Cheat Sheet

[[_TOC_]]

## Setup

Ensure you've followed the [Getting Started](getting-started.md).

## Build Code

```bash
make build
```

### Build Backend

```bash
make -C backendworker build
```

### Build Web

```bash
make -C web build
```

### Build Dev Stubs

```bash
make -C bddtests build
```

## Unit Tests

```bash
make test
```

## Running Code

### Run the latest develop (no need to build locally)

```bash
make run TAG=develop
```

### Run everything built locally

```bash
make run
```

### Run everything built locally using the NHS login dev environment (without this defaults to ext)

```bash
make run LOGINENV=dev
```

### Run everything built locally using the dev stubs

```bash
make run-dev-stubs
```

### Run everything built locally using the minimock (Perf) stubs

```bash
make run-perf-stubs
```

### Run android emulator pointing at local services

```bash
emulator @emulator_name -dns-server 127.0.0.1
```

See [Android - Running in Emulator](Solution-Areas/android.md#running-in-emulator) for more details.

### Debugging web locally

```bash
make run WEB=host
```

### Debugging API locally

```bash
make run PFSAPI=host
```

A `launchsettings.json` file will be automatically generated for any backend API services which are set to run on the host. This will match the specified run configuration. The list of available APIs can be found in the [IMAGE_SETTING_NAMES](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/buildscripts/lib/set_env.sh&version=GBdevelop) variable. Mulitiple may be specifed to debug more than one API concurrently.

### Debugging web locally with develop versions of the rest

```bash
make run TAG=develop WEB=host
```

## Integration Tests (BDD)

### Run Locally

To build and start the application ready to run the Integration tests against (e.g. in IntelliJ)

```bash
make localbdd
```

To start the locally built application ready to run the Integration tests against

```bash
make run-localbdd
```

To start a CI built application version ready to run the Integration tests against

```bash
make run-localbdd TAG=[tag]
```

Where \[tag\] is the CI tag to run, e.g. develop, `3355` (for a PR), or `08cafda6ed4f1ce3bd24ac3ec98810a27ee6f62c` (for a specific commit). By default the latest version of any remote images will be pulled before running. To override this behaviour add `NO_PULL=1` to the make command.

Run make with no arguments for more details on the available options.

### Run Pipeline

To run the Integration tests in a fully containerised environment as is done in CI

```bash
make run-bdd
```

The Makefile in the `bddtests` contains additional targets for common configurations (e.g. running native tests via BrowserStack).

#### Options

The following can be specified with `make run-bdd` to customise the behaviour

| Option             | Description                                                                                                           |
| ---------------    | -----------                                                                                                           |
| `RUN_LOCAL_BDD=1`  | Starts the containers configured as specified but with ports exposed to allow local running of the Integration tests. |
| `SKIP_ANALYSIS=1`  | Bypasses the gradle code analysis step.                                                                               |
| `TAG=[dockertag]`  | Pull images with the specified \[dockertag\] to run the tests against.                                                |
