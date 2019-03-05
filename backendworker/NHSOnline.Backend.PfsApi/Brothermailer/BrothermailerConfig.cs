using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public class BrothermailerConfig: IBrothermailerConfig
    {
        public Uri BrothermailerBaseUrl { get; }
        public string BrothermailerAddressBookId { get; }
        public string BrothermailerSig { get; }
        public string BrothermailerUserId { get; }
        
        public BrothermailerConfig(IConfiguration configuration, ILogger<BrothermailerConfig> logger)
        {
            var brothermailerUriString = configuration.GetOrWarn("BROTHER_MAILER_URL", logger);
          
            if (!string.IsNullOrEmpty(brothermailerUriString))
            {
                BrothermailerBaseUrl = new Uri($"{brothermailerUriString}", UriKind.Absolute);
            }

            BrothermailerAddressBookId = configuration.GetOrWarn("BROTHER_MAILER_ADDRESSBOOK_ID", logger);
            BrothermailerSig = configuration.GetOrWarn("BROTHER_MAILER_SIG", logger);
            BrothermailerUserId = configuration.GetOrWarn("BROTHER_MAILER_USER_ID", logger);
        }
    }
}