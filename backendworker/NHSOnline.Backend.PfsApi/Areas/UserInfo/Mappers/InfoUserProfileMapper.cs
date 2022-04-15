using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.PfsApi.Areas.UserInfo.Mappers
{
    public class InfoUserProfileMapper : IMapper<CitizenIdSessionResult, InfoUserProfile>
    {
        private readonly ILogger _logger;

        public InfoUserProfileMapper(ILogger<InfoUserProfileMapper> logger)
        {
            _logger = logger;
        }

        public InfoUserProfile Map(CitizenIdSessionResult source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            return new InfoUserProfile
            {
                NhsNumber = source.Session.ProofLevel == ProofLevel.P9 ? source.NhsNumber.RemoveWhiteSpace() : null,
                OdsCode = source.Session.OdsCode,
                Email = source.Email
            };
        }
    }
}
