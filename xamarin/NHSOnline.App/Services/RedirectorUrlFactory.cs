using System;
using NHSOnline.App.Config;

namespace NHSOnline.App.Services
{
    internal sealed class RedirectorUrlFactory
    {
        private readonly INhsAppWebConfiguration _configuration;

        public RedirectorUrlFactory(INhsAppWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Uri CreateUrlRedirect(Uri url)
        {
            return new Uri($"{_configuration.RedirectorAddress}?redirect_to={url}");
        }
    }
}