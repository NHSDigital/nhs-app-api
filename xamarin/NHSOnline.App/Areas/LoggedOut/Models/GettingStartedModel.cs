using System;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    internal sealed class GettingStartedModel
    {
        public GettingStartedModel(Uri? deeplinkUrl)
        {
            DeeplinkUrl = deeplinkUrl;
        }

        public Uri? DeeplinkUrl { get; }
    }
}