using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity
{
    public class AssertedLoginSessionVisitor : IUserSessionVisitor<Task<CreateJwtResult>>
    {

        private readonly IAuditor _auditor;
        private readonly IAssertedLoginIdentityService _assertedLoginIdentityService;
        private readonly CreateJwtRequest _model;

        public AssertedLoginSessionVisitor(
            CreateJwtRequest model,
            IAuditor auditor, IAssertedLoginIdentityService assertedLoginIdentityService)
        {
            _auditor = auditor;
            _assertedLoginIdentityService = assertedLoginIdentityService;
            _model = model;
        }

        public Task<CreateJwtResult> Visit(P5UserSession userSession)
        {
            return Task.FromResult(
                _assertedLoginIdentityService.CreateJwtToken(userSession.CitizenIdUserSession.IdTokenJti));
        }

        public async Task<CreateJwtResult> Visit(P9UserSession userSession)
        {
            return await _auditor.Audit()
                .Operation(AuditingOperations.CreateAssertedLoginIdentityToken)
                .Details("Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', " +
                         "Jump Off ID '{2}', intended relying party URL: {3}",
                    _model.ProviderId, _model.ProviderName, _model.JumpOffId, _model.IntendedRelyingPartyUrl)
                .Execute(() =>
                    Task.FromResult(
                        _assertedLoginIdentityService.CreateJwtToken(userSession.CitizenIdUserSession.IdTokenJti)));

        }
    }
}