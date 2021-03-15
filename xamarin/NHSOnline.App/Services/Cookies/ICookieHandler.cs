using System;
using System.Net;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas.Cookies
{
    internal interface ICookieHandler
    {
        Task AddCookies(ICookieView view, Uri cookieDomain, CookieContainer cookies);
    }
}