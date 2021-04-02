using System;
using System.Web;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageMapper : ITppLinkageMapper
    {
        private readonly ILogger<TppLinkageMapper> _logger;

        public TppLinkageMapper(ILogger<TppLinkageMapper> logger)
        {
            _logger = logger;
        }

        public LinkageResponse Map(LinkAccount linkAccount, LinkAccountReply linkAccountReply)
        {
            if (!IsRequestValid(linkAccount))
            {
                throw new ArgumentException("Request is not valid", nameof(linkAccount));
            }

            if (!IsResponseValid(linkAccountReply))
            {
                throw new ArgumentException("Response is not valid", nameof(linkAccountReply));
            }

            var passphraseToLink = linkAccountReply.PassphraseToLink;
            var passphraseToLinkDecoded = HttpUtility.HtmlDecode(passphraseToLink);

            return new LinkageResponse
            {
                AccountId = linkAccountReply.AccountId,
                OdsCode = linkAccount.OrganisationCode,
                LinkageKey = passphraseToLinkDecoded
            };
        }

        private bool IsRequestValid(LinkAccount request)
        {
            if (request == null)
            {
                return false;
            }

            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.OrganisationCode, nameof(request.OrganisationCode))
                .IsValid();
        }

        private bool IsResponseValid(LinkAccountReply addNhsUserResponse)
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
