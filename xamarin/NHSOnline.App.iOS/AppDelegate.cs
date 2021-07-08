using System;
using System.Diagnostics.CodeAnalysis;
using Foundation;
using NHSOnline.App.iOS.DependencyServices.Notifications;
using NHSOnline.App.iOS.Handlers;
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

        private RemoteNotificationReceivedHandler? _remoteNotificationRecievedHandler;

        internal static AppDelegate? Instance { get; private set; }
        private NhsApp? NhsApp { get; set; }

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

            NhsApp = new NhsApp();
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterHandler();

            _remoteNotificationRecievedHandler = new RemoteNotificationReceivedHandler(NhsApp);

            LoadApplication(NhsApp);

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public void SetHandler(RemoteNotificationRegistrationHandler remoteNotificationRegistrationHandler)
            => _remoteNotificationRegistrationHandler = remoteNotificationRegistrationHandler;

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
            => _remoteNotificationRegistrationHandler?.RegisteredForRemoteNotifications(deviceToken);

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
            => _remoteNotificationRegistrationHandler?.FailedToRegisterForRemoteNotifications(error);

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
            => _remoteNotificationRecievedHandler?.DidReceiveRemoteNotification(userInfo);

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity,
            UIApplicationRestorationHandler completionHandler)
        {
            var url = userActivity.WebPageUrl;
            if (url != null)
            {
                NhsApp?.HandleDeeplink(url.AbsoluteString);
            }

            return base.ContinueUserActivity(application, userActivity, completionHandler);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            SecureScreen.Show();
            base.DidEnterBackground(uiApplication);
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
            SecureScreen.Hide();
        }
    }
}
