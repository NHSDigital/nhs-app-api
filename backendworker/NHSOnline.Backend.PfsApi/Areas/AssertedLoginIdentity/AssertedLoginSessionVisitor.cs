using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
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
            IAuditor auditor,
            IAssertedLoginIdentityService assertedLoginIdentityService)
        {
            _auditor = auditor;
            _assertedLoginIdentityService = assertedLoginIdentityService;
            _model = model;
        }

        public async Task<CreateJwtResult> Visit(P5UserSession userSession) => await CreateJwt(userSession);

        public async Task<CreateJwtResult> Visit(P9UserSession userSession)
        {
            return await _auditor.Audit()
                .Operation(AuditingOperations.CreateAssertedLoginIdentityToken)
                .Details("Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', " +
                         "Jump Off ID '{2}', intended relying party URL: {3}",
                    _model.ProviderId, _model.ProviderName, _model.JumpOffId, _model.IntendedRelyingPartyUrl)
                .Execute(async () => await CreateJwt(userSession));
        }

        private Task<CreateJwtResult> CreateJwt(P5UserSession userSession)
        {
            var createJwtResult = _assertedLoginIdentityService.CreateJwtToken(userSession.CitizenIdUserSession.IdTokenJti);
            return Task.FromResult(createJwtResult);
        }
    }
}