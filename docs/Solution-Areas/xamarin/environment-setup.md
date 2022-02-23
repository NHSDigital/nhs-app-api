# Xamarin Environment Setup

## Windows

Install Xamarin "Mobile Development .Net" using Visual Studio Install

## Mac
**Before you begin, make sure you check with one of the Team about the version of Xcode currently in use. Xcode version is tied to MacOS - you may need to upgrade your OS, so do this first.**

### Rider

[Follow Getting started with Xamarin on Rider](https://www.jetbrains.com/help/rider/Xamarin.html#). This will require Visual Studio for Mac to be installed but not used.

### Xcode
Xcode is required to be installed. Kainos users - Xcode can be installed via the Company Portal, but you will be automatically updated upon new releases. If you want to be in control of your XCode Version, you can [download specific versions here](https://developer.apple.com/download/all/) (Note: you may need to raise a ticket with Kainos IT to granted admin rights to install).

### Getting the iOS project to build
- The iOS project has a provisioning profile of the 'NHS Digital' Apple developer account. This means you need to get your @nhs.net account to create an AppleID/account in Apple Developer, then get someone on the team to add you to the Apple Developer 'NHS Digital' team.
- You can confirm you've been added by going to https://appstoreconnect.apple.com/access/users, finding and selecting your user and ensuring you have the following permissions: (you may need someone with a greater level of access to grant these):
  - Access to Certificates, Identifiers & Profiles
  - Access to Cloud Managed Distribution Certificate
- Then you need this profile on your machine for Rider to pick it up. Open `XCode -> Preferences -> Accounts` and sign in with your AppleId. It will pull in your personal role and also the NHS Digital team you are now in. **NB: you must also click the box to 'Download manual profiles'.** 

  ![Xcode - sign in and Download Manual Profiles](Images/XcodeDownloadProfiles.png)

### Running in an iOS simulator (Kainos staff)

For Zscaler to permit traffic from your iOS simulator, you must add the KAINOS-ROOT-CA certificate to it. If you don't, you may receive the error "An SSL error has occurred and a secure connection to the server cannot be made" in Rider. The symptom of this error is that you cannot get past the initial welcome screen and you receive an error "We cannot confirm which version of the NHS App you are using".

Download the certificate to the device.

To do this, you could:
- export your KAINOS-ROOT-CA certificate from keychain to your documents.
- download the certificate to your simulator (you could upload it somewhere secure like your Kainos OneDrive, sign into OneDrive on your simulator and download it from there).
- upon download, you will receive a message asking you to go to settings to install the certificate.
- go to `Settings > General > Device Management`, find the certificate and tap "Install" (you may receive another warning and you may have to tap "Install" again).
- Now you must trust the certificate. Go to `General > About > Certificate Trust settings` and toggle the certificate on.


### Troubleshooting

- You may have to direct Rider to point at the specific Mono version that Xamarin forms requires.

![RiderMonoVersion](Images/RiderMonoVersion.png)
- iOS: when building the App, if you get the error: 'Could not find any available provisioning profiles for NHSOnline.App.iOS', then you haven't clicked 'Download manual profiles' in Xcode.
- iOS: If you have clicked 'Download manual profiles' in Xcode, and its still not building...
  - In Rider, right-click on the iOS project and select `open in XCode`
  - In XCode, select the project and click on the `Signing & Capabilities` tab.
  - In the `Signing` section, open the `Team` dropdown and select `NHS Digital`. That should magically fix the provisioning profiles. ![RiderAndXcode](Images/RiderAndXcode.png)
- iOS: if the App builds, but you cannot debug as there is no iOS device simulator available for selection in Rider (or the dropdown is totally missing):
  - check the `Xcode Path` is set in Rider: `Preferences -> Build,Execution,Deployment -> iOS -> Xcode path` to e.g. `/Applications/Xcode.app`
  - Open the Xcode app, create a default Xamarin forms project targetting iOS. Debug this and wait for the Simulator app to spin up. Restart Rider and try again.
- Android: if you get complaints about the wrong version of the SDK (eg its looking for v29), open `Visual Studio for Mac -> Preferences -> Projects/Sdk Locations/Android`. Find the correct SDK e.g. under `Android 10.0 - Q` then install it too.

## Android

For more Android-specific setup steps, see the [xamarin android docs](android.md)