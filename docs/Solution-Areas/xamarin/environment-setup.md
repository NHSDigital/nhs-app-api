# Xamarin Environment Setup

## Windows

Install Xamarin "Mobile Development .Net" using Visual Studio Install

## Mac
**Before you begin, make sure you check with one of the Team about the version of Xcode currently in use. Xcode version is tied to MacOS - you may need to upgrade your OS, so do this first.**

### Rider

[Follow Getting started with Xamarin on Rider](https://www.jetbrains.com/help/rider/Xamarin.html#). This will require Visual Studio for Mac to be installed but not used.

Xcode is also required to be installed. Kainos users - Xcode has to be installed via the Company Portal.

### Getting the iOS project to build
- The iOS project has a provisioning profile of the 'NHS Digital' Apple developer account. This means you need to get your @nhs.net account to create an AppleID/account in Apple Developer, then get someone on the team to add you to the Apple Developer 'NHS Digital' team.
- Then you need this profile on your machine for Rider to pick it up. Open `XCode -> Preferences -> Accounts` and sign in with your AppleId. It will pull in your personal role and also the NHS Digital team you are now in. **NB: you must also click the box to 'Download manual profiles'.** 

  ![Xcode - sign in and Download Manual Profiles](Images/XcodeDownloadProfiles.png)

### Troubleshooting

- You may have to direct Rider to point at the specific Mono version that Xamarin forms requires.

![RiderMonoVersion](Images/RiderMonoVersion.png)
- iOS: when building the App, if you get the error: 'Could not find any available provisioning profiles for NHSOnline.App.iOS', then you haven't clicked 'Download manual profiles' in Xcode.
- iOS: if the App builds, but you cannot debug as there is no iOS device simulator available for selection in Rider (or the dropdown is totally missing):
  - check the `Xcode Path` is set in Rider: `Preferences -> Build,Execution,Deployment -> iOS -> Xcode path` to e.g. `/Applications/Xcode.app`
  - Open the Xcode app, create a default Xamarin forms project targetting iOS. Debug this and wait for the Simulator app to spin up. Restart Rider and try again.
- Android: if you get complaints about the wrong version of the SDK (eg its looking for v29), open `Visual Studio for Mac -> Preferences -> Projects/Sdk Locations/Android`. Find the correct SDK e.g. under `Android 10.0 - Q` then install it too.

## Android

For more Android-specific setup steps, see the [xamarin android docs](android.md)