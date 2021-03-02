using System;
using System.Threading.Tasks;
using NHSOnline.App.Controls;
using NHSOnline.App.Threading;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    internal class BrowserOverlay : IBrowserOverlay
    {
        public async Task OpenBrowserOverlay(Uri uri)
        {
            await Browser.OpenAsync(uri, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = NhsUkColours.NhsUkBlue,
                PreferredControlColor = NhsUkColours.NhsUkWhite
            }).ResumeOnThreadPool();
        }
    }
}