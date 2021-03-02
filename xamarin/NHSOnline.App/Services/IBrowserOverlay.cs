using System;
using System.Threading.Tasks;

namespace NHSOnline.App.Services
{
    internal interface IBrowserOverlay
    {
        public Task OpenBrowserOverlay(Uri uri);
    }
}