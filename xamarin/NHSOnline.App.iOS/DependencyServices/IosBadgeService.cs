using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosBadgeService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosBadgeService: IBadgeService
    {
        private readonly ILogger _logger = NhsAppLogging.CreateLogger<IosBadgeService>();

        public Task SetBadgeCount(string count)
        {
            try
            {
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = Convert.ToInt32(count, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to convert badge count {Count} to integer", count);
            }

            return Task.CompletedTask;
        }
    }
}