using System.Net;

namespace NHSOnline.App.Areas.PreHome.Models
{
    internal sealed class NhsAppPreHomeScreenWebModel
    {
        internal NhsAppPreHomeScreenWebModel(CookieContainer cookies)
        {
            Cookies = cookies;
        }

        internal CookieContainer Cookies { get; }
    }
}