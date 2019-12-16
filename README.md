# nhsonline-app

> NHS Online App

## Get code

Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-app.git

```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-app.git
```

## Build code

```bash
make build
```

## Unit Tests

```bash
make test
```

## Running code

### Run the latest develop (no need to build locally)

```bash
make run TAG=develop
```

### Run everything built locally

```bash
make run
```

### Debugging web locally

```bash
make run WEB=host
```

### Debugging API locally

```bash
make run PFSAPI=host
```

### Debugging web locally with develop versions of the rest

```bash
make run TAG=develop WEB=host
```

## BDD (Integration) Tests

### Run Locally

To build and start the application ready to run the BDD tests against (e.g. in IntelliJ)

```bash
make localbdd
```

To start the locally built application ready to run the BDD tests against

```bash
make run-localbdd
```

To start a CI built application version ready to run the BDD tests against

```bash
make run-localbdd TAG=[tag]
```

Where \[tag\] is the CI tag to run, e.g. develop, 3355 (for a PR), or 08cafda6ed4f1ce3bd24ac3ec98810a27ee6f62c (for a specific commit). By default the latest version of any remote images will be pulled before running. To override this behaviour add `NO_PULL=1` to the make command.

Run make with no arguments for more details on the available options.

### Run Pipeline

To run the BDD tests in a fully containerised environment as is done in CI

```bash
make run-bdd
```

The Makefile in the `bddtests` contains additional targets for common configurations (e.g. running native tests via BrowserStack).

#### Options

The following can be specified with `make run-bdd` to customise the behaviour

| Option           | Description                                                                                                   |
| ---------------  | -----------                                                                                                   |
| RUN_LOCAL_BDD=1  | Starts the containers configured as specified but with ports exposed to allow local running of the BDD tests. |
| SKIP_PREPARE=1   | Bypasses the gradle prepare step. Useful if restarting the containers and this has already been run.          |
| TAG=[dockertag]  | Pull images with the specified \[dockertag\] to run the tests against.                                        |

> IOS app

## Cocoapod install

In a terminal run the following commands

1. Make sure you have cocoapods on your machine:

    ```bash
    sudo gem install cocoapods
    ```

2. Navigate to the ios/NHSOnline folder

3. Check for any cocoapod updates:

    ```bash
    pod install
    ```

    The cocapod should now install as long as the pods are pulled down in the repo if not run:

    ```bash
    pod update
    ```

4. To see the pod in your project you will need to open the NHSOnline.xcworkspace fiole in xcode instead of the NHSOnline.xcodeproj

see more here: https://guides.cocoapods.org/using/using-cocoapods.html

## Troubleshooting

if when you try to run 'pod update' or 'pod install'you see an error that mentions a target opvveride the 'ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES' build setting then go to the build settings for NHSOnline and in build options change 'Yes' to '$(inherited)'
