using System.Net;

namespace NHSOnline.App.Areas.Home.Models
{
    internal sealed class NhsAppWebModel
    {
        internal CookieContainer Cookies { get; }

        internal NhsAppWebModel(CookieContainer cookies)
        {
            Cookies = cookies;
        }
    }
}