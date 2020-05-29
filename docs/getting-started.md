# nhsapp

[[_TOC_]]

## Get code

Clone the repo: https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp

```bash
git clone https://nhsapp@dev.azure.com/nhsapp/NHS%20App/_git/nhsapp
```

## Commit code

Review the [Git Branching Strategy](https://confluence.service.nhs.uk/display/NO/Git+Branching+Strategy).

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

*Used in:* bddtests, backendworker contracts and web

- Create a new file:

  - `${HOME}/.npmrc` (OSX/Linux)
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

*Used in:* Backendworker

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

*Used in:* android, bddtests

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
2. Add a line to [buildscripts/validate_local_secrets.sh](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/buildscripts/validate_local_secrets.sh&version=GBdevelop) containing the name of the secret. e.g.

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

6. Configure stubbed environment

   If the secret is a certificate or key update [docker/stubbed/docker-compose.yml](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/stubbed/docker-compose.yml&version=GBdevelop) to reference the appropriate files in [docker/stubbed/certs](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/stubbed/certs&version=GBdevelop)

      ```bash
      my_new_secret:
          file: "./docker/stubbed/certs/TestCert.pfx"
      ```

7. Configure Integration Tests

    If a specific value (different to the default) is required for integration tests create a suitable dummy secret file in [docker/bddtests/secrets](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/bddtests/secrets&version=GBdevelop). This must *not* contain a real secret but a dummy value for the secret expected by the tests.

    Update [docker/bddtests/docker-compose.yml](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/bddtests/docker-compose.yml&version=GBdevelop) to reference the dummy secret.

      ```bash
      my_new_secret:
          file: "./docker/bddtests/secrets/my_new_secret"
      ```
