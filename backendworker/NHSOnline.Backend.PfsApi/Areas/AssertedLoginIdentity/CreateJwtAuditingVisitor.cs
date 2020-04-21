using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    public class CreateJwtAuditingVisitor : ICreateJwtResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AssertedLoginIdentityController> _logger;
        private readonly CreateJwtRequest _request;

        private const string AuditType = AuditingOperations.CreateAssertedLoginIdentityTokenResponse;

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public CreateJwtAuditingVisitor(IAuditor auditor, ILogger<AssertedLoginIdentityController> logger,
            CreateJwtRequest request)
        {
            _auditor = auditor;
            _logger = logger;
            _request = request;
        }

        public async Task Visit(CreateJwtResult.Success result)
        {
            try
            {
                var kvp = new Dictionary<string, string>
                {
                    { "ProviderId", _request.ProviderId },
                    { "ProviderName", _request.ProviderName },
                    { "JumpOffId", _request.JumpOffId },
                    { "IntendedRelyingPartyUrl", _request.IntendedRelyingPartyUrl },
                };
                _logger.LogInformationKeyValuePairs("Created Asserted Login Identity", kvp);

                await _auditor.Audit(AuditType, "AssertedLoginIdentity Token creation succeeded.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(CreateJwtResult.Success)}");
            }
        }

        public async Task Visit(CreateJwtResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "AssertedLoginIdentity Token creation failed.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(CreateJwtResult.InternalServerError)}");
            }
        }
    }
}