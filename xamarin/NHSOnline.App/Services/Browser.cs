using System;
using System.Threading.Tasks;
using NHSOnline.App.Threading;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    internal class Browser : IBrowser
    {
        public async Task OpenBrowserOverlay(Uri uri)
        {
            await Xamarin.Essentials.Browser.OpenAsync(uri, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show
            }).ResumeOnThreadPool();
        }

        public async Task OpenExternalBrowser(Uri uri)
        {
            await Xamarin.Essentials.Browser.OpenAsync(uri, BrowserLaunchMode.External).ResumeOnThreadPool();
        }
    }
}