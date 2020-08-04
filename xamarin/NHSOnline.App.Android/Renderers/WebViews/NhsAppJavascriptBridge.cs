using Android.Webkit;
using Java.Interop;
using Newtonsoft.Json;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using Object = Java.Lang.Object;

namespace NHSOnline.App.Droid.Renderers.WebViews
{
    public class NhsAppJavascriptBridge : Object
    {
        public const string JavascriptObjectName = "nativeApp";

        private readonly NhsAppWebView _nhsAppWebView;

        public NhsAppJavascriptBridge(NhsAppWebView nhsAppWebView)
        {
            _nhsAppWebView = nhsAppWebView;
        }

        [JavascriptInterface]
        [Export("openWebIntegration")]
        public void OpenWebIntegration(string argumentJson)
        {
            NhsAppResilience.ExecuteOnMainThread(() =>
            {
                var argument = JsonConvert.DeserializeObject<OpenWebIntegrationRequest>(argumentJson);
                _nhsAppWebView.OpenWebIntegrationCommand.Execute(argument);
            });
        }
    }
}
