using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    public class QualtricsConfig : IQualtricsConfig
    {
        public QualtricsConfig(IConfiguration configuration, ILogger<QualtricsConfig> logger)
        {
            BaseUrl = new Uri(configuration.GetOrThrow("QUALTRICS_BASE_URL", logger));
            DirectoryId = configuration.GetOrThrow("QUALTRICS_DIRECTORY_ID", logger);
            MailingListId = configuration.GetOrThrow("QUALTRICS_MAILING_LIST_ID", logger);
            Token = configuration.GetOrThrow("QUALTRICS_API_KEY", logger);
        }

        public Uri BaseUrl { get; set; }
        public string DirectoryId { get; set; }
        public string MailingListId { get; set; }
        public string Token { get; set; }
    }
}