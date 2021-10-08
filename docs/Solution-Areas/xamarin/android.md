# Xamarin - Android

## Update Path to include emulator

Add the SDK emulator directory (`C:\Users\<username>\AppData\Local\Android\Sdk\emulator` on Windows or `/Users/<username>/Library/Android/sdk/emulator` on mac) to your PATH.

## Development Debug Keystore

In order to build the app, access to a common debug keystore is required. The following files should be copied across by the project build files. If they do not exist, copy manually from keybase:

* `~/.nhsonline/secrets/android-debug-store-file`

## Running in Emulator

The emulator runs in its own network which, by default, uses the DNS servers configured on the host machine. It does not use the `hosts` file from the host machine. Access to the host machine is done through the IP address `10.0.2.2`. In order to route requests to the application running on the host machine (e.g. `web.local.bitraft.io`, `api.local.bitraft.io`) a DNS server is spun up in docker which returns `10.0.2.2` for those hosts (see [android/docker-compose.yml](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=/docker/android/docker-compose.yml&version=GBdevelop)). The emulator is started via the command line to specify that it should use this DNS server.

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

3. Choose the `Local - Debug | Any CPU` build configuration in Xamarin

4. Click the green play button