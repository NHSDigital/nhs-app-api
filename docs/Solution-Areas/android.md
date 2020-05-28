# NHS Online Android

[Setup instructions on Confluence](https://confluence.service.nhs.uk/display/NO/Android)

## Update Path to include emulator

Add the SDK emulator directory (`C:\Users\<username>\AppData\Local\Android\Sdk\emulator` on Windows or `/Users/<username>/Library/Android/sdk/emulator` on mac) to your PATH.

## FIDO Client Library

The Fido client library is pulled in from a maven repository hosted within Azure Artifacts. If you get access denied errors ensure you have followed the [instructions to setup maven artifacts development access](https://confluence.service.nhs.uk/display/NO/Tech+and+Tooling+-+Azure+DevOps+-+Maven+Artifacts).

## Development Debug Keystore

In order to build the app, access to a common debug keystore is required. The following files should be copied across by the project build files. If they do not exist, copy manually from keybase:

* `~/.nhsonline/secrets/android-debug-store-file`
* `~/.nhsonline/secrets/android-development.gradle`

## Running in Emulator

The emulator runs in its own network which, by default, uses the DNS servers configured on the host machine. It does not use the `hosts` file from the host machine. Access to the host machine is done through the IP address `10.0.2.2`. In order to route requests to the application running on the host machine (e.g. `web.local.bitraft.io`, `api.local.bitraft.io`) a DNS server is spun up in docker which returns `10.0.2.2` for those hosts (see [android/docker-compose.yml](/docker/android/docker-compose.yml)). The emulator is started via the command line to specify that it should use this DNS server.

1. Start the emulator

   ```bash
   emulator @pixel_2 -dns-server 127.0.0.1
   ```

   Replace `pixel_2` with the name of the emulator you wish to use (select from `emulator -list-avds`). Make sure you've [updated your path to include the SDK emulator](#update-path-to-include-emulator).

2. Start the web and backend configured for android app

   ```bash
   make run-android
   ```

   or (to use the dev stubs)

   ```bash
   make run-android-stubs
   ```

3. Set the Active Build Variant in Android Studio to be `localHttp`

4. Click the green play button

## Debugging on device with mocks

Connect to a network which doesn't prevent other devices on the same network from accessing its public IP e.g. a mobile hotspot (many work networks are restricted and won't allow you to do this).

Obtain the IP address of the machine you are running the application from (using `ifconfig`/`ipconfig`).

Update all IP address in the following files (replace 10.0.2.2 with your IP address).

* android/app/src/debug/res/values/configstrings.xml
* bddtests/docker-compose.local-appium.yml

Update `bddtests/src/main/kotlin/config/Config.kt` variable name `cidHostname` (or set env variable `CID_HOST`) to be your machine IP address.

From bddtests run:

```bash
docker-compose -f docker-compose.yml -f docker-compose.local-appium.yml up
```
