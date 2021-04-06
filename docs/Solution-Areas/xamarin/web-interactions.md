# Xamarin Web Interactions

The various web views need to interact with the Xamarin native code. Javascript functions are exposed for the web to call ([Web to Xamarin](#web-to-xamarin)) and the Xamarin code calls functions defined in the web ([Xamarin to Web](#xamarin-to-web)).

## Serialisation

Any parameters to these function calls need to be converted from JSON to C# objects or vice versa. This conversion is always done in the appropriate WebView using classes defined in the `Payloads` namespace. These classes should just be used for passing the data from the WebView to the presenter or from the presenter to the WebView. Any services used by the presenter should have their own interfaces decoupled from the javascript payloads. This reduces the risk of inadverently changing something that breaks the javascript interaction.

## Web to Xamarin

For calls from the Web into the Xamarin code the WebView should expose:

* A command for the page to bind to
* A method for the platform specific renderer to call

```csharp
        public void OpenWebIntegration(string json)
        {
            var request = ConvertFromJsonString<OpenWebIntegrationRequest>(json);
            OpenWebIntegrationCommand.Execute(request);
        }

        public AsyncCommand<OpenWebIntegrationRequest> OpenWebIntegrationCommand
        {
            get => (AsyncCommand<OpenWebIntegrationRequest>) GetValue(OpenWebIntegrationCommandProperty);
            set => SetValue(OpenWebIntegrationCommandProperty, value);
        }
```

The platform specific code then exposes the function to the pages in the webview. When the function is called the approriate method on the WebView is called, passing the argument if any.

```csharp
        [JavascriptInterface]
        [Export("openWebIntegration")]
        public void OpenWebIntegration(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() => _nhsAppWebView.OpenWebIntegration(argumentJson));
        }
```

```csharp
            _javascriptBridge = JavascriptBridge
                .ForWebView(() => (NhsAppWebView) Element, "nativeApp")
                .AddFunction("openWebIntegration", webView => webView.OpenWebIntegration)
                ...
                .Apply(config.UserContentController);
```

## Xamarin to Web

For calls from the Xamarin code into the Web the WebView should expose a method that takes the appropriate payload class. This method is responsible for serialising the payload to JSON and invoking the appropriate web callback method.

```csharp
        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
        {
            const string callbackName = "window.nativeAppCallbacks.notificationsAuthorised";
            var argumentJson = ConvertToJsonString(authorisedResponse);
            await EvaluateJavaScriptAsync($"{callbackName}({argumentJson})").ResumeOnThreadPool();
        }
```

The page containing the WebView then exposes this method to the presenter, delegating its implementation to the WebView:

```csharp
        public async Task SendNotificationAuthorised(NotificationAuthorisedResponse authorisedResponse)
            => await WebView.SendNotificationAuthorised(authorisedResponse).ResumeOnThreadPool();
```

The presenter determines that the callback should be invoked, creates the payload object and calls the method on the view.

```csharp
                var response = new NotificationAuthorisedResponse(
                    trigger,
                    authorisedResult.DevicePns,
                    authorisedResult.DeviceType);

                await _view.SendNotificationAuthorised(response).PreserveThreadContext();
```
