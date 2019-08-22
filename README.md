# nhsonline-app

> NHS Online App

## Get code
Clone from the GitLab repo: https://git.nhschoices.net/nhsonline/nhsonline-app.git

```bash
git clone https://git.nhschoices.net/nhsonline/nhsonline-app.git
```

2. Copy docker-compose.override.yml (sets VISION_CERT_PASSPHRASE env variable) from keybase root folder into:
  - backendworker folder
  - web folder

## Pipeline Tests
**NOTE:**
* Docker 17.05 or higher on the daemon and client is required (for example [docker4mac](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac)).
* Delete existing node_module folder if you already run `npm install` locally to prevent issues starting container with incompatible libraries

To run the pipeline tests in a fully containerised environment execute the build and test script

```bash
./build_and_test.sh
```

It can optionally be called with reference to a specific test tag e.g.

```bash
./build_and_test.sh @prescription
```

for full usage information for the script launch with the -h switch

```bash
./build_and_test.sh -h
```

> IOS app

## Cocoapod install
In a terminal run the following commands

1. Make sure you have cocoapods on your machine:
sudo gem install cocoapods

2. Navigate to the ios/NHSOnline folder

3. Check for any cocoapod updates:
```
pod install
```
The cocapod should now install as long as the pods are pulled down in the repo if not run:
```
pod update
```
4. To see the pod in your project you will need to open the NHSOnline.xcworkspace fiole in xcode instead of the NHSOnline.xcodeproj

see more here: https://guides.cocoapods.org/using/using-cocoapods.html

## Troubleshooting
if when you try to run 'pod update' or 'pod install'you see an error that mentions a target opvveride the 'ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES' build setting then go to the build settings for NHSOnline and in build options change 'Yes' to '$(inherited)'

