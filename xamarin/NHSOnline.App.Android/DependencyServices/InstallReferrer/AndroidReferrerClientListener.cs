using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using Xamarin.Android.InstallReferrer;

namespace NHSOnline.App.Droid.DependencyServices.InstallReferrer
{
    internal class AndroidReferrerClientListener : Java.Lang.Object, IInstallReferrerStateListener
        {
            private readonly InstallReferrerClient? _referrerClient;
            private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidReferrerClientListener));

            public AndroidReferrerClientListener(InstallReferrerClient referrerClient)
            {
                _referrerClient = referrerClient;
            }

            public void OnInstallReferrerServiceDisconnected()
            {
                Logger.LogInformation("Connection to referrer client successfully closed");
            }

            [SuppressMessage("Design", "CA1725: Parameter names should match base declaration",
                Justification = "p0 is not a descriptive name for the parameter")]
            public void OnInstallReferrerSetupFinished(int referrerResponseCode)
            {
                switch (referrerResponseCode)
                {
                    case InstallReferrerClient.InstallReferrerResponse.Ok:
                        Logger.LogInformation("Successfully got response from referrer request");

                        if (_referrerClient == null)
                        {
                            Logger.LogError("ReferrerClient not initialized");
                            break;
                        }
                        var referrerDetails = _referrerClient.InstallReferrer;

                        if (referrerDetails == null)
                        {
                            Logger.LogError("Failed to obtain referrer details from install referrer");
                            break;
                        }

                        AndroidAppReferrerState.AppReferrer = referrerDetails.InstallReferrer;
                        break;

                    case InstallReferrerClient.InstallReferrerResponse.DeveloperError:
                        Logger.LogError("Failed referrer request for a developer error");
                        break;

                    case InstallReferrerClient.InstallReferrerResponse.PermissionError:
                        Logger.LogError("Failed referrer request for a permission error");
                        break;

                    case InstallReferrerClient.InstallReferrerResponse.ServiceDisconnected:
                        Logger.LogError("Failed referrer request for a service disconnected error");
                        break;

                    case InstallReferrerClient.InstallReferrerResponse.ServiceUnavailable:
                        Logger.LogError("Failed referrer request for a service unavailable error");
                        break;

                    case InstallReferrerClient.InstallReferrerResponse.FeatureNotSupported:
                        Logger.LogError("Failed referrer request for a feature not supported error");
                        break;
                    default:
                        break;
                }

                _referrerClient?.EndConnection();
            }
        }
}