using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
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

        public override void SetContentView(View? view)
        {
            base.SetContentView(view);

            MakeNotFocusableIfViewGroup(view);
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// The Xamarin platform creates a couple of ViewGroups on which it sets Focusable to be true.
        /// This causes keyboard navigation to focus the two ViewGroups on tab before the elements of the page.
        /// Here we set Focusable to be false the initial content view and any view groups added to it.
        ///
        /// https://github.com/xamarin/Xamarin.Forms/issues/12631
        /// </summary>
        private static void MakeNotFocusableIfViewGroup(View? view)
        {
            if (view is ViewGroup viewGroup)
            {
                MakeNotFocusable(viewGroup);
            }
        }

        private static void MakeNotFocusable(ViewGroup viewGroup)
        {
            viewGroup.Focusable = false;
            
            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                MakeNotFocusableIfViewGroup(viewGroup.GetChildAt(i));
            }
            
            viewGroup.ChildViewAdded += (_, args) => MakeNotFocusableIfViewGroup(args.Child);
        }
    }
}