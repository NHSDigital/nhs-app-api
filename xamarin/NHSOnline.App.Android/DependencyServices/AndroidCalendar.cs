using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCalendar))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidCalendar: ICalendar
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidCalendar));

        public void AddToCalendar(AddEventToCalendarRequest request)
        {
            Logger.LogInformation("Android calendar implementation not done yet");
        }

        public void ShowAlertPopup()
        {
            Logger.LogInformation("Android calendar implementation not done yet");
        }

        public Task<bool> RequestPermission()
        {
            Logger.LogInformation("Android calendar implementation not done yet");
            return (Task<bool>) Task.CompletedTask;
        }
    }
}