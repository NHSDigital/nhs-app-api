using System.Net;
using NHSOnline.App.Api.Session;

namespace NHSOnline.App.Areas.Home.Models
{
    internal sealed class NhsAppWebModel
    {
        internal NhsAppWebModel(UserSession userSession, CookieContainer cookies)
        {
            UserSession = userSession;
            Cookies = cookies;
        }

        internal UserSession UserSession { get; }

        internal CookieContainer Cookies { get; }
    }
}