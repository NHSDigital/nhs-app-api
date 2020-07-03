using System.Net.Http;
using NHSOnline.App.Api;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosHttpMessageHandlerFactory))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosHttpMessageHandlerFactory : IPrimaryHttpMessageHandlerFactory
    {
        // NSUrlSessionHandler is the recommended handler:
        // https://docs.microsoft.com/en-us/xamarin/cross-platform/macios/http-stack#nsurlsession
        public HttpMessageHandler CreatePrimaryHttpMessageHandler() => new NSUrlSessionHandler();
    }
}