NHS Online Android
==============

# Pre-Requisites

The web app is currently running and accessible via localhost.  If you are planning to use an emulator on windows, this must be done using [docker-toolbox](https://docs.docker.com/toolbox/toolbox_install_windows/) and the port forwarding script found in the [nhsonline-dev-utils repo](https://git.nhschoices.net/nhsonline/nhsonline-dev-utils).

# Submodules

The Fido Client is pulled in from a maven repository hosted within gitlab. It has been added as a submodule to this repository.
In the event that the submodule has not been pulled / updated then you will need to run the following commands:

```
git submodule update
cd android/maven
git pull
```

This will ensure that the maven submodule has an up to date copy of it's repository. 

# Backend-Worker

In the docker-compose.yml file, set the NHS_WEB_APP_BASE_URL variable to the following:
    http://10.0.2.2:3000/
Run the backend-worker locally using Docker, navigate to the backend workers directory and run the following commands:
    'docker-compose build'
    'docker-compose up'
The backend-worker should now be running on docker
This can be confirmed by performing the 
    'docker ps'
To bring down all running processes on docker use 
    'docker-compose down'

# Mobile-Web
Set the variables as shown in the env.js file:
    API_HOST = 'http://10.0.2.2:8082'
    CID_REDIRECT_URI = 'http://10.0.2.2:3000/auth-return'
To run the mobile web project locally, ensure you have done an
    'npm install'
After that, the project can be run using
    'npm run dev'

# Building

Set the variables in the configstrings.xml to the following: 
    <string name="baseScheme">http</string>
    <string name="baseHost">10.0.2.2</string>
    <string name="basePort">3000</string>
    <string name="authRedirectPath">/auth-return</string>
    <string name="baseURL">http://10.0.2.2:3000/</string>
and in configstrings.xml(debug)
    <string name="baseURL">http://10.0.2.2:3000/</string>
In the AndroidManifest.xml file ensure that the <data> section looks like the following:
    <data
        android:scheme="@string/baseScheme"
        android:host="@string/baseHost"
        android:pathPrefix="@string/authRedirectPath"
        android:port="@string/basePort"/>

Build the app using Android Studio.  The app acts as a shell for the web app.

See the page on Confluence for more detail.

# Debugging on device with mocks

Connect to a network which doesn't prevent other devices on the same network from accessing its public IP e.g. a mobile hotspot (many work networks are restricted and won't allow you to do this).

Obtain the IP address of the machine you are running the application from (using ifconfig).

Update all IP address in the following files (replace 10.0.2.2 with your IP address).

* android/app/src/debug/res/values/configstrings.xml
* bddtests/docker-compose.local-appium.yml

Update `bddtests/src/main/kotlin/config/Config.kt` variable name `cidHostname` (or set env variable `CID_HOST`) to be your machine IP address.

From bddtests run:

```
docker-compose -f docker-compose.yml -f docker-compose.local-appium.yml up
```
