using System;

namespace NHSOnline.App.Controls.WebViews
{
    public sealed class StartNhsLoginUpliftRequest
    {
        public StartNhsLoginUpliftRequest(Uri url)
        {
            Url = url;
        }

        public Uri Url { get; set; }
    }
}