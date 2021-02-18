# Xamarin Integration Tests

[[_TOC_]]

## Keyboard Accessibility

### Keyboard Accessibility - Android

The class `AndroidKeyboardNavigation` is used to navigate a page using the keyboard.

For each page we:

* assert that the tab key can be used to cycle through all the appropriate elements
* verify that the keyboard can be used to interact with all approrpriate elements once focused via the tab key (go to link, toggle check box, click button etc.)

The [UI Automator viewer](https://developer.android.com/training/testing/ui-automator#ui-automator-viewer) tool can be used to identify which elements of a page are focusable and which currently have the focus.

![UI Automator viewer](Images/integration-tests-ui-automator-viewer.png)

### Keyboard Accessibility - iOS

We have not found anyway to automate testing of keyboard accessibility on iOS
