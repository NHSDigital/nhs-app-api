using System.Net.Http;
using NHSOnline.App.Api;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Android.Net;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidHttpMessageHandlerFactory))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidHttpMessageHandlerFactory : IPrimaryHttpMessageHandlerFactory
    {
        public HttpMessageHandler CreatePrimaryHttpMessageHandler()
        {
            return new AndroidClientHandler();
        }
    }
}