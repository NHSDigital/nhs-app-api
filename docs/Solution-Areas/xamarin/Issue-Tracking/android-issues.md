# Xamarin

## Issue Tracking - Android

### WebView links

#### Problem

Through the use of the stubbed login page it was noticed a consistent failure in the integration test using the internal error page which should open a new tab. This was done through an additional `target="_blank"` in the link. This test was consistently failing only on android as the link click did nothing.

This was found to be an open issue with Xamarin forms 4.8.0.1687 which has not been resolved yet in later versions (https://github.com/xamarin/Xamarin.Forms/issues/12917)

The bug description which matched our issue was as follows:
``` Xamarin Forms v4.8.0.1364 update addresses a spoofing vulnerability by setting SetSupportMultipleWindows(true) to prevent the malicious Javascript from running in the WebView. This setting causes the WebView not responding to Navigating and Navigated events in some common cases that need to handle these events. ```

#### Workaround

The workaround suggested on the open issue was to add `webView.Settings.SetSupportMultipleWindows(false);` in onChanged for any webview that we are creating.