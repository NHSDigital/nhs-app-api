using System;

namespace NHSOnline.App.Areas.LoggedOut.Models
{
    public class BeginLoginModel
    {
        internal Uri? DeeplinkUrl { get; }
        public string? FidoAuthResponse { get; }

        internal BeginLoginModel(Uri? deeplinkUrl, string? fidoAuthResponse)
        {
            DeeplinkUrl = deeplinkUrl;
            FidoAuthResponse = fidoAuthResponse;
        }
    }
}