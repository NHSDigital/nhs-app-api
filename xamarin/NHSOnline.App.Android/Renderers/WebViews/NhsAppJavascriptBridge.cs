using Android.Webkit;
using Java.Interop;
using Java.Lang;
using NHSOnline.App.Controls.WebViews;

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
        [Export("navigateToThirdParty")]
        public void NavigateToThirdParty(string thirdParty)
        {
            _nhsAppWebView.Dispatcher.BeginInvokeOnMainThread(
                () => _nhsAppWebView.NavigateToThirdPartyCommand.Execute(thirdParty));
        }
    }
}
