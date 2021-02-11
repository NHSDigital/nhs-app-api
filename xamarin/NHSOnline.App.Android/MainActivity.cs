using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using NHSOnline.App.Droid.DependencyServices;

namespace NHSOnline.App.Droid
{
    [Activity(Label = "NHSOnline.App", Theme = "@style/MainTheme", Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_launcher_round", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
#if DEBUG_WEBVIEW
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
#endif

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            AndroidLifecycle.MainActivity = this;

            LoadApplication(new NhsApp());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}