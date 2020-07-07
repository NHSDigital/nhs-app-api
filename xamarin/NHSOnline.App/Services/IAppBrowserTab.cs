using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Services
{
    internal interface IAppBrowserTab
    {
        public Task OpenAppBrowserTab(Uri uri);
    }
}