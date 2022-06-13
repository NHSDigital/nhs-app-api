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
            var storeIntentResult = OpenIntent("market://details?id=com.nhs.online.nhsonline");
            if (storeIntentResult == null)
            {
                Logger.LogInformation("Successfully launched play store store from market url");
                return Task.CompletedTask;
            }

            var browserIntentResult = OpenIntent("https://play.google.com/store/apps/details?id=com.nhs.online.nhsonline");
            if (browserIntentResult == null)
            {
                Logger.LogInformation("Successfully launched play store store from browser url");
                return Task.CompletedTask;
            }

            Logger.LogError("Failed to open play store and browser intents. {ErrorMessage}\n{AdditionalErrorMessage}",
                    storeIntentResult.Message, browserIntentResult.Message);

            return Task.CompletedTask;
        }

        private static ActivityNotFoundException? OpenIntent(string uriIntent)
        {
            using var intent = new Intent(Intent.ActionView);
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