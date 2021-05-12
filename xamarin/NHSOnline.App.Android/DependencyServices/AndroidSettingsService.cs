using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidSettingsService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidSettingsService : ISettingsService
    {
        internal static MainActivity? MainActivity { get; set; }

        public Task OpenSettings()
        {
            using var intent = new Intent(
                Settings.ActionApplicationDetailsSettings,
                Android.Net.Uri.Parse($"package:{Android.App.Application.Context.PackageName}"));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.NoHistory);
            MainActivity?.StartActivity(intent);
            return Task.CompletedTask;
        }
    }
}