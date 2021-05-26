using System.Diagnostics.CodeAnalysis;
using Android.App;
using Android.Content;
using AndroidX.AppCompat.App;
using NHSOnline.App.Droid.Extensions;

namespace NHSOnline.App.Droid
{
    public class SplashScreenActivity : AppCompatActivity
    {
        [SuppressMessage("ReSharper", "CA2000",
            Justification = "The intent is passed through to Android where it will be collected")]
        protected override void OnResume()
        {
            base.OnResume();

            var mainActivity = new Intent(Application.Context, typeof(MainActivity));

            if (Intent?.DataString != null)
            {
                mainActivity.AddDeepLink(Intent.DataString);
            }

            if (Intent?.Extras != null)
            {
                mainActivity.PutExtras(Intent.Extras);
            }

            StartActivity(mainActivity);
        }
    }
}