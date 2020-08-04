using System;
using Foundation;
using Newtonsoft.Json;
using NHSOnline.App.Controls;
using WebKit;

namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal static class ScriptMessageHandler
    {
        public static IWKScriptMessageHandler For(Func<AsyncCommand> command) => new NoArguments(command);
        public static IWKScriptMessageHandler For(Func<AsyncCommand<string>> command) => new StringArgument(command);
        public static IWKScriptMessageHandler For<TArgument>(Func<AsyncCommand<TArgument>> command) => new JsonArgument<TArgument>(command);

        private sealed class NoArguments : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<AsyncCommand> _command;

            internal NoArguments(Func<AsyncCommand> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                NhsAppResilience.ExecuteOnMainThread(ExecuteCommand);
            }

            private void ExecuteCommand()
            {
                _command().Execute(null);
            }
        }

        private sealed class StringArgument : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<AsyncCommand<string>> _command;

            public StringArgument(Func<AsyncCommand<string>> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                NhsAppResilience.ExecuteOnMainThread(() => ExecuteCommand(message.Body.ToString()));
            }

            private void ExecuteCommand(string argument)
            {
                _command().Execute(argument);
            }
        }

        private sealed class JsonArgument<TArgument> : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<AsyncCommand<TArgument>> _command;

            public JsonArgument(Func<AsyncCommand<TArgument>> command) => _command = command;

            public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                NhsAppResilience.ExecuteOnMainThread(() => ExecuteCommand(message.Body.ToString()));
            }

            private void ExecuteCommand(string argumentJson)
            {
                var argument = JsonConvert.DeserializeObject<TArgument>(argumentJson) ??
                               throw new ArgumentException($"Failed to deserialise JSON to {typeof(TArgument).FullName}", nameof(argumentJson));

                _command().Execute(argument);
            }
        }
    }
}