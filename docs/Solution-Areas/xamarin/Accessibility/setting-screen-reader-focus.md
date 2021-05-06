# Xamarin - Setting screen reader focus

In order to set screen reader focus, platform specific functions must be called. As a result, when a particular type of control requires focus, a custom Renderer will need to be created to handle this behaviour. As a general flow:

- declare an event handler in the view/control that needs focus
- create an event handler with the same signature in the appropriate renderer
- override `OnElementChanged` in the renderer and have it detach/attach the handler to the element
- where appropriate, invoke the handler

## Setting focus

### Android

```C#
private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
{
    Control.PerformAccessibilityAction(Action.AccessibilityFocus, null);
}
```

### iOS

```C#
private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
{
    UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, Control);
}
```

### Non-native root elements

When the control you want to focus has a root element of, for example, `ContentView` (or any other type that has no underlying native `Control`), you'll need to get a reference to the nested element's native control and send the event from there.

Using `Heading1Renderer` as an example, which has a root element of `ContentView`:

- Add the Xamarin element type and underlying native types to the base renderer

  ### Android
  
  ```C#
  class Heading1Renderer : ViewRenderer<ContentView, Android.Views.View>
  ```
  
  ### iOS
  
  ```C#
  class Heading1Renderer : ViewRenderer<ContentView, UIView>
  ```

- Instead send the accessibility event on the child native element

  ### Android
  
  ```C#
  private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
  {
      var textView = Element.Content.GetRenderer()?.View;
      textView?.PerformAccessibilityAction(Action.AccessibilityFocus, null);
  }
  ```
  
  ### iOS
  
  ```C#
  private void OnAccessibilityFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
  {
      if (Element.Content.GetRenderer()?.NativeView is NSObject uiView)
      {
          UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, uiView);
      }
  }
  ```

## Headings

As a result of a Nomensa review, when a new page is loaded, the level 1 heading on that page needs to receive focus. When creating a new page in Xamarin, simply call `Heading1.AccessibilityFocus` on the heading control in `View.OnAppearing`.