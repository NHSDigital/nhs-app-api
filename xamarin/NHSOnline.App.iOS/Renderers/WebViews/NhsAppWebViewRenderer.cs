using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Foundation;
using Newtonsoft.Json;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.iOS.Renderers.WebViews;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsAppWebView), typeof(NhsAppWebViewRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal static class ScriptMessageHandler
    {
        public static IWKScriptMessageHandler For(Func<ICommand> command) => new NoArguments(command);
        public static IWKScriptMessageHandler For(Func<Command<string>> command) => new StringArgument(command);
        public static IWKScriptMessageHandler For<TArgument>(Func<Command<TArgument>> command) => new JsonArgument<TArgument>(command);

        private sealed class NoArguments : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<ICommand> _command;

            internal NoArguments(Func<ICommand> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                _command().Execute(null);
            }
        }

        private sealed class StringArgument : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<Command<string>> _command;

            public StringArgument(Func<Command<string>> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                var argument = message.Body.ToString();
                _command().Execute(argument);
            }
        }

        private sealed class JsonArgument<TArgument> : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<Command<TArgument>> _command;

            public JsonArgument(Func<Command<TArgument>> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                var argumentJson = message.Body.ToString();
                var argument = JsonConvert.DeserializeObject<TArgument>(argumentJson);
                _command().Execute(argument);
            }
        }
    }
    internal sealed class NhsAppWebViewRenderer : WkWebViewRenderer
    {
        private const string JavaScriptFunction = @"window.nativeApp = {};
                                                    window.nativeApp.navigateToThirdParty = function(url) {
                                                        window.webkit.messageHandlers.navigateToThirdParty.postMessage(url);
                                                    }";

        private readonly NSString _javascriptFunction;
        private readonly WKUserContentController _userController;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public NhsAppWebViewRenderer() : this(CustomConfiguration)
        {
        }

        private NhsAppWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            _userController = config.UserContentController;
            _javascriptFunction = new NSString(JavaScriptFunction);
            var script = new WKUserScript(_javascriptFunction, WKUserScriptInjectionTime.AtDocumentStart, false);
            _userController.AddUserScript(script);
            _disposables.Add(script);
            var handler = ScriptMessageHandler.For(() => ((NhsAppWebView)Element).NavigateToThirdPartyCommand);
            _userController.AddScriptMessageHandler(handler, "navigateToThirdParty");
            _disposables.Add(handler);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement == null)
            {
                NavigationDelegate = new DelegatingWebViewNavigationDelegate(NavigationDelegate);

            }

            if (e.OldElement is NhsAppWebView oldNhsAppWebView)
            {
                oldNhsAppWebView.SetCookie = null;
            }

            if (e.NewElement is NhsAppWebView newNhsAppWebView)
            {
                newNhsAppWebView.SetCookie = SetCookie;
            }

            base.OnElementChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.ForEach(d => d.Dispose());
                _javascriptFunction.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task SetCookie(Cookie cookie)
        {
            using var nsHttpCookie = new NSHttpCookie(cookie);

            await Configuration.WebsiteDataStore.HttpCookieStore.SetCookieAsync(nsHttpCookie).ConfigureAwait(true);
        }

        private static WKWebViewConfiguration CustomConfiguration => new WKWebViewConfiguration
        {
            ApplicationNameForUserAgent = " nhsapp-ios/1.0.0",
            SuppressesIncrementalRendering = true
        };
    }
}
