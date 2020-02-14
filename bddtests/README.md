# NHSApp Integration Tests

The NHSApp integration tests describe and verify the behaviour of the application. They are written in feature files using cucumber syntax with Kotlin for the steps implementation.

See also:

* [Automated BDD Testing](https://confluence.service.nhs.uk/display/NO/Automated+BDD+Testing)
* [BDD Standards](https://confluence.service.nhs.uk/display/NO/BDD+Standards)

## Integration Tests Setup

The integration tests execute against the application running in docker started by docker-compose. In addition to the application a number of supporting services are started including mongo db and wiremock.

The configuration overrides for the application can be found in `/docker/bddtests`. The `Makefile` and scripts in the `buildscripts` directory are used to start the application and services such that the integration tests may be run.

The integration tests themselves can be run either through an IDE (e.g. IntelliJ) or in a docker container (to more closely replicate the CI environment).

## Running Integration Tests

By default all of the `make run` commands will execute the tests in docker. To start the services so that the tests can be executed on the host machine append `RUN_LOCAL_BDD=1` to the command line.

It is possible to start the docker containers with specific services omitted and requests for that service routed to the host machine. See the [root README](/README.md) for more details.

### Run Default Configuration

```bash
make run
```

This will run the smoketests with the default integration tests configuration. To run set of tests with a specific tag append `TRANCHE_TAG=tag-to-run`, e.g.

```bash
make run TRANCHE_TAG=organ-donation
```

### Run Custom Configuration

Some integration tests require specific configuration which would conflicit with the sucessful execution of other tests. For these there are separate make targets.

e.g. to run the accessibility tests (and generate the pa11y report:

```bash
make run-accessibility
```

To get a list of the available configurations run `make`:

```bash
$ make
run                            Run the integration tests
run-accessibility              Run the accessibility integration tests
run-cosmos                     Run the cosmos integration tests
run-onlineconsultations        Run the online consultations integration tests
run-others                     Run the other integration tests
run-throttling                 Run the throttling integration tests
run-local                      Run in docker with ports open
run-native-android             Run the nativesmoketests in BrowserStack against Android
run-native-ios                 Run the nativesmoketests in BrowserStack against iOS; NATIVE_APP_PATH_IOS must be set to the path to the ipa to test
help                           This help.
```
