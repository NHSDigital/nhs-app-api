using System;
using System.Diagnostics.CodeAnalysis;
using Android.App;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using Xamarin.Android.InstallReferrer;

namespace NHSOnline.App.Droid.DependencyServices.InstallReferrer
{
    internal sealed class AndroidInstallReferrerService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidInstallReferrerService));
        internal static Activity MainActivity { get; set; } = null!;

        [SuppressMessage("Design", "CA2000: Dispose objects before losing scope",
            Justification = "Object passed to Android systems for disposal")]
        internal static void CreateReferrerClientAndStoreDetails()
        {
            if (!string.IsNullOrEmpty(AndroidAppReferrerState.AppReferrer))
            {
                return;
            }

            var referrerClient = InstallReferrerClient.NewBuilder(MainActivity).Build();
            Logger.LogInformation("Starting referrer client connection to store details");
            try
            {
                var referrerListener = new AndroidReferrerClientListener(referrerClient);
                referrerClient.StartConnection(referrerListener);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to start referrer client connection", e);
            }
        }
    }
}