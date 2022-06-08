using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Services
{
    internal interface IBrowser
    {
        public Task OpenBrowserOverlay(Uri uri);

        public Task OpenExternalBrowser(Uri uri);
    }
}