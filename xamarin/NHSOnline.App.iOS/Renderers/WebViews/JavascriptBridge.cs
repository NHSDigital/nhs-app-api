using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Foundation;
using NHSOnline.App.Controls;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal static class JavascriptBridge
    {
        public static JavascriptBridge<TWebView> ForWebView<TWebView>(Func<TWebView> webViewAccessor)
            => new JavascriptBridge<TWebView>(webViewAccessor);
    }

    internal sealed class JavascriptBridge<TWebView>: IDisposable
    {
        private readonly Dictionary<string, IWKScriptMessageHandler> _functions = new Dictionary<string, IWKScriptMessageHandler>();

        private readonly Func<TWebView> _webViewAccessor;

        private WKUserScript? _shimJavascriptScript;
        private NSString? _shimJavascript;

        // The webview does not exist at the point we instantiate the bridge, and so we pass an
        // accessor to ensure we can get hold of the instance at the point of command execution
        internal JavascriptBridge(Func<TWebView> webViewAccessor)
        {
            _webViewAccessor = webViewAccessor;
        }

        public JavascriptBridge<TWebView> AddFunction(string name, Func<TWebView, AsyncCommand> command)
        {
            _functions.Add(name, ScriptMessageHandler.For(() => command(_webViewAccessor())));
            return this;
        }

        public JavascriptBridge<TWebView> AddFunction(string name, Func<TWebView, AsyncCommand<string>> command)
        {
            _functions.Add(name, ScriptMessageHandler.For(() => command(_webViewAccessor())));
            return this;
        }

        public JavascriptBridge<TWebView> AddFunction<TArgument>(string name, Func<TWebView, AsyncCommand<TArgument>> command)
        {
            _functions.Add(name, ScriptMessageHandler.For(() => command(_webViewAccessor())));
            return this;
        }

        public JavascriptBridge<TWebView> Apply(WKUserContentController userContentController)
        {
            foreach (var (name, handler) in _functions)
            {
                userContentController.AddScriptMessageHandler(handler, name);
            }

            _shimJavascript = new NSString(GenerateShimJavascript());
            _shimJavascriptScript = new WKUserScript(_shimJavascript, WKUserScriptInjectionTime.AtDocumentStart, false);
            userContentController.AddUserScript(_shimJavascriptScript);

            return this;
        }

        private string GenerateShimJavascript()
        {
            var shimJavascript = new StringBuilder("window.nativeApp = {};");

            foreach (var (name, _) in _functions)
            {
                shimJavascript.AppendFormat(
                    CultureInfo.InvariantCulture,
                    @"
window.nativeApp.{0} = function(request) {{
    window.webkit.messageHandlers.{0}.postMessage(request);
}}",
                    name);
            }

            return shimJavascript.ToString();
        }

        public void Dispose()
        {
            foreach (var handler in _functions.Values)
            {
                handler.Dispose();
            }

            _shimJavascript?.Dispose();
            _shimJavascriptScript?.Dispose();
        }
    }
}