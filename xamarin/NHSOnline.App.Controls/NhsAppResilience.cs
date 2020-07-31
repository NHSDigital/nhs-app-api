using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NHSOnline.App.Controls
{
    public static class NhsAppResilience
    {
        private static Action<Exception> _attemptRecovery = AttemptRecoveryNotInitialised;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(NhsAppResilience));

        public static void Init(INavigation navigation, IDispatcher dispatcher)
        {
            _attemptRecovery = e => AttemptRecovery(e, dispatcher, navigation);
        }

        public static void ExecuteOnMainThread(Action action)
        {
            MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        AttemptRecovery(e);
                    }
                });
        }

        public static void ExecuteOnMainThread(Func<Task> action)
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    await action().ConfigureAwait(true);
                }
                catch (Exception e)
                {
                    AttemptRecovery(e);
                }
            });
        }

        public static void AttemptRecovery(Exception e) => _attemptRecovery(e);

        private static void AttemptRecovery(Exception e, IDispatcher dispatcher, INavigation navigation)
        {
            Logger.LogError(e, "Attempting recovery from unanticipated exception");

            dispatcher.BeginInvokeOnMainThread(async () =>
            {
                await navigation.PopToRootAsync().ConfigureAwait(true);
                if (navigation.NavigationStack[0] is IRootPage rootPage)
                {
                    rootPage.ResetAndShowError();
                }
            });
        }

        private static void AttemptRecoveryNotInitialised(Exception e)
        {
            try
            {
                Logger.LogError(e, $"Unable to attempt recovery as not {nameof(NhsAppResilience)} is not initialised");
            }
            catch
            {
                DependencyService.Get<INativeLog>().Log(
                    LogLevel.Critical,
                    nameof(NhsAppResilience),
                    "Unable to attempt recovery as not initialised");
            }
        }
    }
}
