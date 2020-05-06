# NHS Online Android

[Setup instructions on Confluence](https://confluence.service.nhs.uk/display/NO/Android)

## FIDO Client Library

The Fido client library is pulled in from a maven repository hosted within Azure Artifacts. If you get access denied errors ensure you have followed the [instructions to setup maven artifacts development access](https://confluence.service.nhs.uk/display/NO/Tech+and+Tooling+-+Azure+DevOps+-+Maven+Artifacts).

## Development Debug Keystore

In order to build the app, access to a common debug keystore is required. The following files should be copied across by the project build files. If they do not exist, copy manually from keybase:

* `~/.nhsonline/secrets/android-debug-store-file`
* `~/.nhsonline/secrets/android-development.gradle`

## Running in Emulator

The emulator uses the IP address 10.0.2.2 to access the host machine. To support this a DNS entry `android.local.bitraft.io` has been created pointing at 10.0.0.2.

1. Start the web and backend configured for android app

   ```bash
   make run-android
   ```

   or

   ```bash
   make run-android-https
   ```

2. Set the Active Build Variant in Android Studio to be `localHttp` or `localHttps` (to match the `make run-` comand above)

3. Select (or create) an virtual device.

   If running with HTTPS ensure you have installed your local dev certificate onto the device:

   1. Drag and drop \$HOME/.nhsonline/local-development-certificate/local-development-https.crt into the emulator
   2. Navigate to Settings \> Security and Location \> Encryption and Credentials \> Install from SD Card
   3. Select the certificate that was just copied to the phone \(in downloads\)
   4. Give the certificate a name, say "LocalDevCert"
   5. Follow the instructions to set a pin \(a fingerprint is not needed\)

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
