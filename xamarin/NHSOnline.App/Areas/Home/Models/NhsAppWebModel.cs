
using System;

namespace NHSOnline.App.Areas.Home.Models
{
    internal sealed class NhsAppWebModel
    {
        internal NhsAppWebModel(Uri? deeplinkUrl)
        {
            DeeplinkUrl = deeplinkUrl;
        }

        internal Uri? DeeplinkUrl { get; }
    }
}