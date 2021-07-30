using System;
using NHSOnline.App.Api.Client.Cookies;

namespace NHSOnline.App.Areas.PreHome.Models
{
    internal sealed class NhsAppPreHomeScreenWebModel
    {
        internal NhsAppPreHomeScreenWebModel(CookieJar cookieJar, Uri? deeplinkUrl)
        {
            CookieJar = cookieJar;
            DeeplinkUrl = deeplinkUrl;
        }

        internal CookieJar CookieJar { get; }
        internal Uri? DeeplinkUrl { get; }
    }
}