# nhsapp

> NHS App

## Get code

Clone the repo: https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp

```bash
git clone https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp
```

## Build code

Before running this for the first time, read the `Azure DevOps Feeds` section below to configure your local environment.

```bash
make build
```

## Commit code

Review the [Git Branching Strategy](https://confluence.service.nhs.uk/display/NO/Git+Branching+Strategy).

## Unit tests

```bash
make test
```

## Running code

Before running any of the below options for the first time, read the `Secrets` section below to configure your local environment.

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

### Debugging web locally

```bash
make run WEB=host
```

### Debugging API locally

```bash
make run PFSAPI=host
```

A `launchsettings.json` file will be automatically generated for any backend API services which are set to run on the host. This will match the specified run configuration.

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

## Azure DevOps Feeds

NPM, NuGet and some Gradle packages (Android/BDD Tests) are pulled from DevOps feeds. These proxy internet sources, prevent tampering of packages and avoid build failures during downtime of services such as NPM.

### Personal Access Token

You require a token to access the feeds, to generate a new one:

- Login in Azure DevOps
- Go to `User Settings` (top-right beside help)
- Click `Personal access tokens`
- Click `New Token`
- Configure `Name` and `Expiration`
- Ensure `Scopes` is set to `Custom defined`
- Check `Packaging > Read`
- Click `Create`
- Add the new token to your password manager

### NPM Config

*Used in: bddtests, backendworker contracts and web*

- Create a new file:

    - `${HOME}/.npmrc ` (OSX/Linux)
    - `%USERPROFILE%\.npmrc` (Windows)

- Add the following content:

    ```c
    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/registry/:username=nhsapp
    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/registry/:_password=<BASE_64_ENCODED_TOKEN>
    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/registry/:email=<HSCIC_EMAIL>

    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/:username=nhsapp
    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/:_password=<BASE_64_ENCODED_TOKEN>
    //pkgs.dev.azure.com/nhsapp/70d4deb2-d387-4e09-b546-da927c2186b9/_packaging/nhsapp-npm-registry/npm/:email=<HSCIC_EMAIL>
    ```

- Create a Base64 encoded version of your personal access token:

    - Open a bash terminal in the directory containing the above file
    - Run:

        ```bash
        echo -n "<TOKEN>" | base64
        ```
    - Copy your encoded token

- Fill in the placeholders `<BASE_64_ENCODED_TOKEN>` & `<HSCIC_EMAIL>`

### NuGet Config

*Used in: Backendworker*

- Create a new file:

    - `${HOME}/.nuget/NuGet/NuGet.Config` (OSX/Linux)
    - `%APPDATA%\NuGet\NuGet.Config` (Windows)

- Add the following content:

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
      </packageSources>

      <packageSourceCredentials>
        <nhsapp-nuget-feed>
          <add key="Username" value="nhsapp" />
          <add key="ClearTextPassword" value="<TOKEN>" />
        </nhsapp-nuget-feed>
      </packageSourceCredentials>
    </configuration>  
    ```

- Fill in the placeholder `<TOKEN>`

## Maven/Gradle Config 

*Used in: android, bddtests*

- Create a new file:

    - `${HOME}/.m2/settings.xml` (OSX/Linux)
    - `%USERPROFILE%\.m2\settings.xml` (Windows)

- Add the following content:

    ```xml  
    <settings xmlns="http://maven.apache.org/SETTINGS/1.0.0"
            xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xsi:schemaLocation="http://maven.apache.org/SETTINGS/1.0.0 http://maven.apache.org/xsd/settings-1.0.0.xsd">
      <servers>
          <server>
          <id>nhsapp</id>
          <username>nhsapp</username>
          <password>TOKEN</password>
          </server>
      </servers>
    </settings>
    ```

- Fill in the placeholder `TOKEN`

## Secrets

Secrets required for running the app locally are stored in Keybase (`team/nhsonline/development_secrets`). These are automatically copied to your home directory (`~/.nhsonline/secrets`) when the app is run using make if the keybase filesystem integration is working. If it is not an error will be reported and the files should be manually copied across.

### Adding Secrets

1. Add a file containing the secret to the `development_secrets` directory in keybase
2. Add a line to `buildscripts/validate_local_secrets.sh` containing the name of the secret. e.g.

    ```bash
    validate_secret my_new_secret
    ```

3. Add the secret to the top of the root `docker-compose.yml` file and include it in the `secrets` section of any services that require it. e.g.

    ```bash
    my_new_secret:
        file: "~/.nhsonline/secrets/my_new_secret"

    services:
        my.service:
            secrets:
                - my_new_secret
    ```

4. Update the appropriate `.env` file in the `docker` directory to set the envrionment variable expected to contain the secret. e.g.

    ```bash
    MY_NEW_SECRET=/run/secrets/my_new_secret
    ```

5. If the envrionment variable is expected to contain the secret and not a path to the secret update the entrypoint for the appropriate services to set the variable from the contents of the file. e.g.

    ```bash
    entrypoint:
      - /bin/bash
      - -c
      - |
          export MY_NEW_SECRET=$$(<$$MY_NEW_SECRET)
          exec $$*
      - "--"
    ```

6. Create a suitable dummy secret file in `bddtests/dummysecrets`. This must *not* contain a real secret but a dummy value for the secret expected by the tests.
7. Update `bddtests/docker-compose.yml` to reference the dummy secret. e.g.

    ```bash
    my_new_secret:
        file: "./bddtests/dummysecrets/my_new_secret"
    ```
