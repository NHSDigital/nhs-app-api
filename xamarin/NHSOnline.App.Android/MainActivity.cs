using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Droid.DependencyServices.Biometrics;
using NHSOnline.App.Droid.DependencyServices.InstallReferrer;
using NHSOnline.App.Droid.Dialogs;
using NHSOnline.App.Droid.Extensions;
using NHSOnline.App.Droid.Handlers;
using Xamarin.Essentials;

namespace NHSOnline.App.Droid
{
    [Activity(Label = "NHSOnline.App",
        LaunchMode = LaunchMode.SingleTask,
        Theme = "@style/MainTheme",
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.FullUser)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private NhsApp? NhsApp { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
#if DEBUG_WEBVIEW
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Window.SetDefaultFlags();

            Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            AndroidLifecycle.MainActivity = this;
            AndroidBiometrics.MainActivity = this;
            AndroidSettingsService.MainActivity = this;
            AlertDialogBox.MainActivity = this;
            AndroidFileHandler.MainActivity = this;
            AndroidInstallReferrerService.MainActivity = this;

            NhsApp = new NhsApp();

            LoadApplication(NhsApp);

            AndroidInstallReferrerService.CreateReferrerClientAndStoreDetails();

            HandleIntent(Intent);
        }

        public override void SetContentView(View? view)
        {
            base.SetContentView(view);

            MakeNotFocusableIfViewGroup(view);
        }

        protected override void OnStop()
        {
            CookieManager.Instance?.Flush();

            base.OnStop();
        }

        protected override void OnDestroy()
        {
            NhsApp?.AppClosing();

            base.OnDestroy();
        }

        protected override void OnPause()
        {
            Window.AddSecureFlag();

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            Window.ClearSecureFlag();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            HandleIntent(intent);
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            OnReceivedGeolocationPermissionsResultHandler.Instance.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// The Xamarin platform creates a couple of ViewGroups on which it sets Focusable to be true.
        /// This causes keyboard navigation to focus the two ViewGroups on tab before the elements of the page.
        /// Here we set Focusable to be false the initial content view and any view groups added to it.
        ///
        /// https://github.com/xamarin/Xamarin.Forms/issues/12631
        /// </summary>
        private const int ExpectedDepthOfPlatformCreatedViewGroupsToMakeNotFocusable = 4;

        private static void MakeNotFocusableIfViewGroup(View? view, int depth = 0)
        {
            if (view is ViewGroup viewGroup)
            {
                MakeNotFocusable(viewGroup, depth);
            }
        }

        private static void MakeNotFocusable(ViewGroup viewGroup, int depth)
        {
            viewGroup.Focusable = false;

            if (depth == ExpectedDepthOfPlatformCreatedViewGroupsToMakeNotFocusable)
            {
                return;
            }

            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                MakeNotFocusableIfViewGroup(viewGroup.GetChildAt(i), depth + 1);
            }

            viewGroup.ChildViewAdded += (_, args) => MakeNotFocusableIfViewGroup(args.Child, depth + 1);
        }

        private void HandleIntent(Intent? intent)
        {
            if (intent == null)
            {
                return;
            }

            var url = intent.GetDeepLink();
            if (!string.IsNullOrWhiteSpace(url))
            {
                NhsApp?.HandleDeeplink(url!);
                Log.Info(nameof(MainActivity), $"Notification contains url {url}");
            }
        }
    }
}