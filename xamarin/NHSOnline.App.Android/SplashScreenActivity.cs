using System.Diagnostics.CodeAnalysis;
using Android.App;
using Android.Content;
using Android.Support.V7.App;

namespace NHSOnline.App.Droid
{
    [Activity(Theme = "@style/MainTheme.SplashScreen", Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_launcher_round", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        [SuppressMessage("ReSharper", "CA2000",
            Justification = "The intent is passed through to Android where it will be collected")]
        protected override void OnResume()
        {
            base.OnResume();

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}