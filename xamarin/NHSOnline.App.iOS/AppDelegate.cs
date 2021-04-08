using System;
using System.Diagnostics.CodeAnalysis;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.iOS.DependencyServices.Notifications;
using NHSOnline.App.iOS.Handlers;
using NHSOnline.App.Logging;
using UIKit;
using UserNotifications;

namespace NHSOnline.App.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "iOS naming convention")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private RemoteNotificationRegistrationHandler? _remoteNotificationRegistrationHandler;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AppDelegate));
        internal static AppDelegate? Instance { get; private set; }

        public AppDelegate()
        {
            Instance = this;
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            Xamarin.Forms.Forms.Init();
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterHandler();

            LoadApplication(new NhsApp());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public void SetHandler(RemoteNotificationRegistrationHandler remoteNotificationRegistrationHandler)
            => _remoteNotificationRegistrationHandler = remoteNotificationRegistrationHandler;

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
            => _remoteNotificationRegistrationHandler?.RegisteredForRemoteNotifications(deviceToken);

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
            => _remoteNotificationRegistrationHandler?.FailedToRegisterForRemoteNotifications(error);

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Will be used for NHSO-13864 when we deal with links in the notifications
            Logger.LogInformation("RECEIVED NOTIFICATION");
        }
    }
}
