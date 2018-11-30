using System;
using System.Web;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageMapper : ITppLinkageMapper
    {
        private readonly ILogger<TppLinkageMapper> _logger;

        public TppLinkageMapper(ILogger<TppLinkageMapper> logger)
        {
            _logger = logger;
        }


        public LinkageResponse Map(AddNhsUserRequest addNhsUserRequest, AddNhsUserResponse addNhsUserResponse)
        {
            if (!IsRequestValid(addNhsUserRequest))
            {
                throw new ArgumentNullException(nameof(addNhsUserRequest));
            }
            
            if (!IsResponseValid(addNhsUserResponse))
            {
                throw new ArgumentNullException(nameof(addNhsUserResponse));
            }

            var passphraseToLink = addNhsUserResponse.PassphraseToLink;
            var passphraseToLinkDecoded = HttpUtility.HtmlDecode(passphraseToLink);

            return new LinkageResponse
            {
                AccountId = addNhsUserResponse.AccountId,
                OdsCode = addNhsUserRequest.OrganisationCode,
                LinkageKey = passphraseToLinkDecoded
            };
        }

        private bool IsRequestValid(AddNhsUserRequest addNhsUserRequest)
        {
            if (addNhsUserRequest == null)
            {
                return false;
            }

            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addNhsUserRequest.OrganisationCode, nameof(addNhsUserRequest.OrganisationCode))
                .IsValid();
        }

        private bool IsResponseValid(AddNhsUserResponse addNhsUserResponse)
        {
            if (addNhsUserResponse == null)
            {
                return false;
            }

            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(addNhsUserResponse.PassphraseToLink, nameof(addNhsUserResponse.PassphraseToLink))
                .IsNotNullOrWhitespace(addNhsUserResponse.AccountId, nameof(addNhsUserResponse.AccountId))
                .IsValid();
        }
    }
}