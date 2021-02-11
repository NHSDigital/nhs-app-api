using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;

[assembly: Xamarin.Forms.Dependency(typeof(IosLifecycle))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosLifecycle: ILifecycle
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosLifecycle));

        public void CloseApplication()
        {
            Logger.LogError("Close application attempt denied as this is not supported in iOS");
        }
    }
}