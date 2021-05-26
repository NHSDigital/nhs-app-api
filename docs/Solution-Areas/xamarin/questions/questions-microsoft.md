# Xamarin Questions for Microsoft

## General

Is it possible to make a bindable property required?

Cookies in iOS appear to be shared between the webview and the HTTP client, (See NHSO-14141) whereas Android does not. How can we get Android to behave like iOS, or iOS to behave like Android?

Why would a cookie with a matching subdomain inconsistently be added to a HTTP call to an endpoint on that domain?

## Android

How should the disposing of objects passed into Android classes be handled?
e.g. [here](https://dev.azure.com/nhsapp/NHS%20App/_git/nhsapp?path=%2Fxamarin%2FNHSOnline.App.Android%2FAndroidServices%2FAndroidNotificationMessagingService.cs&version=GBdevelop&line=61&lineEnd=63&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents)

Is it possible to debug a running Android or iOS app? This is useful and important when the app is opened by a user actioning a claimed url or a notification.

Is it possible to detect that the webview has finished redering the page to allow us to trigger accessibility functions to direct focus to the appropriate part of the page? Or some other guidance on directing focus within the webview.

The activity around the intent filter includes the splash screen information:
<activity android:icon="@mipmap/ic_launcher" android:noHistory="true" android:roundIcon="@mipmap/ic_launcher_round" android:theme="@style/MainTheme.SplashScreen" android:name="crc64b7e535562fc6760f.SplashScreenActivity">
Does the android name change?