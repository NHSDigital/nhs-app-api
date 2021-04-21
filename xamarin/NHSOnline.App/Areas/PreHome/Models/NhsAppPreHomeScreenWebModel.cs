using System;
using System.Net;

namespace NHSOnline.App.Areas.PreHome.Models
{
    internal sealed class NhsAppPreHomeScreenWebModel
    {
        internal NhsAppPreHomeScreenWebModel(CookieContainer cookies, Uri? deeplinkUrl)
        {
            Cookies = cookies;
            DeeplinkUrl = deeplinkUrl;
        }

        internal CookieContainer Cookies { get; }
        internal Uri? DeeplinkUrl { get; }
    }
}