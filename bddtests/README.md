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

e.g. to run the accessibility tests (and generate the pa11y report):

```bash
make run-accessibility
```

To get a list of the available configurations run `make`:

```bash
$ make
run                            Run the integration tests
run-accessibility              Run the accessibility integration tests
run-onlineconsultations        Run the online consultations integration tests
run-others                     Run the other integration tests
run-throttling                 Run the throttling integration tests
run-local                      Run in docker with ports open
run-native-android             Run the nativesmoketests in BrowserStack against Android
run-native-ios                 Run the nativesmoketests in BrowserStack against iOS; NATIVE_APP_PATH_IOS must be set to the path to the ipa to test
help                           This help.
```

## BrowserStack Native Tests

BrowserStack App Automate (https://app-automate.browserstack.com/) is used to execute the integration tests against the app running on a device (iOS or Android).

### App Location

In order to run the tests the app must be uploaded to BrowserStack. Both iOS and android apps have specific BrowserStack bulid configurations containing the appropriate URLs for the integration tests.

#### App Location - Android

The environment variable `NATIVE_APP_PATH_ANDROID` should point to the apk to upload. This defaults to `android/app/build/outputs/apk/browserstack/app-browserstack-unsigned.apk` which is where the android build (`make -C android build`) places the browserstack apk.

#### App Location - iOS

The environment variable `NATIVE_APP_PATH_IOS` should point to the ipa to upload. To get an ipa to test it needs to be extracted from XCode ("archive" then "export") or downloaded from a build in Azure DevOps.

### BrowserStack Local

Running the appropriate `make` configuration (`run-native-android` or `run-native-ios`) will startup the [BrowserStack Local](https://www.browserstack.com/local-testing/app-automate) binary in a container alongside the App containers. The BrowserStack Local binary proxies requests from the native device running on BrowserStack's service to the local services running in docker. The `BROWSERSTACK_LOCAL_IDENTIFIER` setting is used to connect the test runner to the appropriate BrowserStack Local instance.

### BrowserStack in IntelliJ

In order to run the integration tests in IntelliJ a number of configuration settings need to be setup. It is recommended that a specific build configuration is setup for both Android and iOS BrowserStack tests, based on the standard configuration.

#### BrowserStack in IntelliJ - Android

Add the following "VM options":

```bash
-Dwebdriver.provided.type=browserstack_android
-Dappium.platformName=ANDROID
```

Add the following environment variables:

| Name                          | Description                                                                                                       |
| ------------------------------| ----------------------------------------------------------------------------------------------------------------- |
| BROWSERSTACK_USERNAME         | Your BrowserStack username                                                                                        |
| BROWSERSTACK_ACCESS_KEY       | Your BrowserStack access key                                                                                      |
| BROWSERSTACK_LOCAL_IDENTIFIER | BrowserStack Local instance id - the build script defaults this to `int_test_$HOSTNAME` (e.g. `int_test_LPT4128`) |
| BROWSERSTACK_DEVICE_NAME      | Device to test - the build script defaults this to `Google Pixel 2`                                               |
| BROWSERSTACK_OS_VERSION       | OS Version to test - the build script defaults this to `8.0`                                                      |
| APP_PATH                      | Path to uploaded app in BrowserStack - the build script uploads to `$HOSTNAME-android` (e.g. `LPT4128-android`)   |
| APP_SCHEME                    | Set to `nhsapp`                                                                                                   |
| AUTOLOGIN                     | Set to `true`                                                                                                     |
| XPATH_PAGE_SOURCE             | Set to `false`                                                                                                    |
| SESSION_EXPIRY_MINUTES        | Set to 3                                                                                                          |

To start the service to test the following command should be used (substituting your BrowserStack access key and username where approriate):

```bash
make run-native-android BROWSERSTACK_USERNAME="Your Username" BROWSERSTACK_ACCESS_KEY="Your Access Key" RUN_LOCAL_BDD=1
```

If you have used a different value to any of the defaults in the table above these should also be specified on the command line. e.g. to use a different device:

```bash
make run-native-android BROWSERSTACK_USERNAME="Your Username" BROWSERSTACK_ACCESS_KEY="Your Access Key" BROWSERSTACK_DEVICE_NAME="Nexus 5" RUN_LOCAL_BDD=1
```

#### BrowserStack in IntelliJ - iOS

Add the following "VM options":

```bash
-Dwebdriver.provided.type=browserstack_ios
-Dappium.platformName=iOS
```

Add the following environment variables:

| Name                          | Description                                                                                                       |
| ------------------------------| ----------------------------------------------------------------------------------------------------------------- |
| BROWSERSTACK_USERNAME         | Your BrowserStack username                                                                                        |
| BROWSERSTACK_ACCESS_KEY       | Your BrowserStack access key                                                                                      |
| BROWSERSTACK_LOCAL_IDENTIFIER | BrowserStack Local instance id - the build script defaults this to `int_test_$HOSTNAME` (e.g. `int_test_LPT4128`) |
| BROWSERSTACK_DEVICE_NAME      | Device to test - the build script defaults this to `iPhone 8`                                                     |
| BROWSERSTACK_OS_VERSION       | OS Version to test - the build script defaults this to `12.1`                                                     |
| APP_PATH                      | Path to uploaded app in BrowserStack - the build script uploads to `$HOSTNAME-ios` (e.g. `LPT4128-ios`)           |
| APP_SCHEME                    | Set to `nhsapp`                                                                                                   |
| AUTOLOGIN                     | Set to `true`                                                                                                     |
| XPATH_PAGE_SOURCE             | Set to `false`                                                                                                    |
| SESSION_EXPIRY_MINUTES        | Set to 3                                                                                                          |

To start the service to test the following command should be used (substituting your BrowserStack access key and username where approriate):

```bash
make run-native-ios BROWSERSTACK_USERNAME="Your Username" BROWSERSTACK_ACCESS_KEY="Your Access Key" RUN_LOCAL_BDD=1
```

If you have used a different value to any of the defaults in the table above these should also be specified on the command line. e.g. to use a different device:

```bash
make run-native-ios BROWSERSTACK_USERNAME="Your Username" BROWSERSTACK_ACCESS_KEY="Your Access Key" BROWSERSTACK_DEVICE_NAME="iPhone X" RUN_LOCAL_BDD=1
```
