using System;
using Foundation;
using NHSOnline.App.Controls;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal static class ScriptMessageHandler
    {
        public static IWKScriptMessageHandler For(Func<Action> action) => new ActionMessageHandler(action);
        public static IWKScriptMessageHandler For(Func<Action<string>> action) => new StringActionMessageHandler(action);

        private sealed class ActionMessageHandler : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<Action> _action;

            internal ActionMessageHandler(Func<Action> action) => _action = action;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                NhsAppResilience.ExecuteOnMainThread(() => _action()());
            }
        }
        
        private sealed class StringActionMessageHandler : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<Action<string>> _action;

            public StringActionMessageHandler(Func<Action<string>> action) => _action = action;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                NhsAppResilience.ExecuteOnMainThread(() => _action()(message.Body.ToString()));
            }
        }
    }
}