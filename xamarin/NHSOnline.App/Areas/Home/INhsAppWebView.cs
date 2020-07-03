using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home
{
    internal interface INhsAppWebView
    {
        event EventHandler Appearing;

        INavigation Navigation { get; }
        void GoToUri(Uri uri);

        Task AddCookie(Cookie cookie);
    }
}