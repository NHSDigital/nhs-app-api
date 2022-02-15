using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidUpdateService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidUpdateService : IUpdateService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<AndroidUpdateService>();

        public Task OpenAppStoreUrl()
        {
            var storeIntentResult = OpenPlayStoreIntent("market://details?id=com.nhs.online.nhsonline");
            if (storeIntentResult == null)
            {
                return Task.CompletedTask;
            }

            var browserIntentResult = OpenIntent("https://play.google.com/store/apps/details?id=com.nhs.online.nhsonline");
            if (browserIntentResult == null)
            {
                Logger.LogError("Failed to open play store intent. {ErrorMessage}", storeIntentResult.Message);
            }
            else
            {
                Logger.LogError("Failed to open play store and browser intents. {ErrorMessage}\n{AdditionalErrorMessage}",
                    storeIntentResult.Message, browserIntentResult.Message);
            }

            return Task.CompletedTask;
        }

        private static ActivityNotFoundException? OpenPlayStoreIntent(string uriIntent)
        {
            // de.androidpit.app is the package name for the Play Store
            var intent = Android.App.Application.Context.PackageManager?.GetLaunchIntentForPackage("de.androidpit.app");
            if (intent == null)
            {
                return new ActivityNotFoundException("Play store was not found by package name de.androidpit.app");
            }

            return OpenIntent(uriIntent, intent);
        }

        private static ActivityNotFoundException? OpenIntent(string uriIntent, Intent? launchIntent = null)
        {
            using var intent = launchIntent ?? new Intent(Intent.ActionView);
            intent.SetData(Uri.Parse(uriIntent));
            intent.AddFlags(ActivityFlags.NewTask);
            try
            {
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (ActivityNotFoundException e)
            {
                return e;
            }
            return null;
        }
    }
}