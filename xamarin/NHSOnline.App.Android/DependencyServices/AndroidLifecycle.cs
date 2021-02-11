using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidLifecycle))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidLifecycle : ILifecycle
    {
        internal static MainActivity? MainActivity { get; set; }

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidLifecycle));

        public void CloseApplication()
        {
            Logger.LogInformation("Back button pressed on Android device, backgrounding the application");
            MainActivity?.Finish();
        }
    }
}