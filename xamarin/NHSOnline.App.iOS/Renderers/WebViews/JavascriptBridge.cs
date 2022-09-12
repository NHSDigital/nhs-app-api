using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal class JavascriptBridge
    {
        public static JavascriptBridge<TWebView> ForWebView<TWebView>(Func<TWebView> webViewAccessor, string javascriptObjectName)
            => new JavascriptBridge<TWebView>(webViewAccessor, javascriptObjectName);
    }

    internal sealed class JavascriptBridge<TWebView>: IDisposable
    {
        private readonly Dictionary<string, IWKScriptMessageHandler> _functions = new Dictionary<string, IWKScriptMessageHandler>();

        private readonly Func<TWebView> _webViewAccessor;

        private WKUserScript? _shimJavascriptScript;
        private NSString? _shimJavascript;

        private readonly string _javascriptObjectName;

        private static ILogger Logger => NhsAppLogging.CreateLogger<JavascriptBridge>();

        // The webview does not exist at the point we instantiate the bridge, and so we pass an
        // accessor to ensure we can get hold of the instance at the point of command execution
        internal JavascriptBridge(Func<TWebView> webViewAccessor, string javascriptObjectName)
        {
            _webViewAccessor = webViewAccessor;
            _javascriptObjectName = javascriptObjectName;
        }

        public JavascriptBridge<TWebView> AddFunction(string name, Func<TWebView, Action> action)
        {
            try
            {
                _functions.Add(name, ScriptMessageHandler.For(() => action(_webViewAccessor())));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error adding Javascript Function (action).\n Function Name: {0} \nWebView: {1}", name,
                    _javascriptObjectName);
                throw;
            }
            return this;
        }

        public JavascriptBridge<TWebView> AddFunction(string name, Func<TWebView, Action<string>> command)
        {
            try
            {
                _functions.Add(name, ScriptMessageHandler.For(() => command(_webViewAccessor())));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error adding Javascript Function (command).\n Function Name: {0} \nWebView: {1}", name,
                    _javascriptObjectName);
                throw;
            }
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
            var shimJavascript = new StringBuilder($"window.{_javascriptObjectName} = {{}};");

            foreach (var (name, _) in _functions)
            {
                shimJavascript.AppendFormat(
                    CultureInfo.InvariantCulture,
                    @"
window.{0}.{1} = function(request) {{
    window.webkit.messageHandlers.{1}.postMessage(request);
}}",
                    _javascriptObjectName,
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
