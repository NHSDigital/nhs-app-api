> IOS app

## Building locally
If xcode displays a message of
>"NHSOnline" requires a provisioning profile with the Push Notifications feature."

Please check the box for
>Automatically enable signing

This will allow Xcode to correctly resolve the project permissions

## Cocoapod install
In a terminal run the following commands

1. Make sure you have cocoapods on your machine:
        sudo gem install cocoapods

2. Navigate to the ios/NHSOnline folder

3. Check for any cocoapod updates:
        pod update
    The cocapod should now install as long as the pods are pulled down in the repo if not run:
        pod install
4. To see the pod in your project you will need to open the NHSOnline.xcworkspace fiole in xcode instead of the NHSOnline.xcodeproj

see more here: https://guides.cocoapods.org/using/using-cocoapods.html

## Troubleshooting
if when you try to run 'pod update' or 'pod install'you see an error that mentions a target opvveride the 'ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES' build setting then go to the build settings for NHSOnline and in build options change 'Yes' to '$(inherited)'
