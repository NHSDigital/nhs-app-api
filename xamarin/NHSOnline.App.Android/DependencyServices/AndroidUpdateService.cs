using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidUpdateService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidUpdateService : IUpdateService
    {
        public Task OpenAppStoreUrl()
        {
            using var openAppStoreIntent = new Intent(Intent.ActionView);

            openAppStoreIntent.SetData(Uri.Parse("market://details?id=com.nhs.online.nhsonline"));
            openAppStoreIntent.AddFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(openAppStoreIntent);

            return Task.CompletedTask;
        }
    }
}