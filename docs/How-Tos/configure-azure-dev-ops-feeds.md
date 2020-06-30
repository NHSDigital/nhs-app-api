# Configure Azure DevOps Feeds

NPM, NuGet and some Gradle packages (Android/BDD Tests) are pulled from DevOps feeds. These proxy internet sources, prevent tampering of packages and avoid build failures during downtime of services such as NPM.

[[_TOC_]]

## Create Personal Access Token

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

## Automatically Configure Feeds

All the feeds can be configured by running the command `make configure-package-feed` from the root of the repo. You will be prompted for parameters when needed.

## Manually Configure Feeds

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

- Fill in the placeholder `<BASE_64_ENCODED_TOKEN>` with the encoded token & `<HSCIC_EMAIL>`

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

- Fill in the placeholder with the actual token `<TOKEN>`

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

- Fill in the placeholder with the actual token `TOKEN`
