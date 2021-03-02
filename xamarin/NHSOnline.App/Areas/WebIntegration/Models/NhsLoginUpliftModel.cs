using System;

namespace NHSOnline.App.Areas.WebIntegration.Models
{
    internal sealed class NhsLoginUpliftModel
    {
        internal NhsLoginUpliftModel(Uri url)
        {
            Url = url;
        }
        
        internal Uri Url { get; }
    }
}