# Xamarin

## Issue Tracking - iOS

### Browserstack unable to find elements

#### Problem

Some tests needed to be ignored due to the inability to find elements on the screen (in this case the covid banner on the login screen). The covid banner was in the page source, but there was nothing to distinguish it, and we could not see the label text (https://app-automate.browserstack.com/dashboard/v2/builds/05473d067ee0c9ba062635914c740ecac3a5b4fb/sessions/3fb9aef4e6b2b575b8dd3b4f8fe087e28fa8c9f5).

#### Solution

The solution to this problem is to ensure you place the label element, rather than in the surrounding layout, in the accessible tree using `AutomationProperties.IsInAccessibleTree="true"` so that this is picked up and the text can be seen in the iOS page source.