using System.Threading.Tasks;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBadgeService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidBadgeService: IBadgeService
    {
        //22769 Android out of scope
        public Task SetBadgeCount(string count)
        {
            return Task.CompletedTask;
        }
    }
}