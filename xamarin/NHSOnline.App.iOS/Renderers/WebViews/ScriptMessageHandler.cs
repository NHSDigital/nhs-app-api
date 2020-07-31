using System;
using System.Windows.Input;
using Foundation;
using Newtonsoft.Json;
using WebKit;
using Xamarin.Forms;

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
                NhsAppResilience.ExecuteOnMainThread(ExecuteCommand);
            }

            private void ExecuteCommand()
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
                NhsAppResilience.ExecuteOnMainThread(() => ExecuteCommand(message.Body.ToString()));
            }

            private void ExecuteCommand(string argument)
            {
                _command().Execute(argument);
            }
        }

        private sealed class JsonArgument<TArgument> : NSObject, IWKScriptMessageHandler
        {
            private readonly Func<Command<TArgument>> _command;

            public JsonArgument(Func<Command<TArgument>> command) => _command = command;

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