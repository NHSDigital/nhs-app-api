using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;

namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public class AssertedLoginIdentityService: IAssertedLoginIdentityService
    {
        private readonly ILogger<AssertedLoginIdentityService> _logger;
        private readonly ICitizenIdJwtHelper _citizenIdJwtHelper;

        public AssertedLoginIdentityService( ICitizenIdJwtHelper citizenIdJwtHelper, ILogger<AssertedLoginIdentityService> logger)
        {
            _citizenIdJwtHelper = citizenIdJwtHelper;
            _logger = logger;
        }

        public CreateJwtResult CreateJwtToken(string idTokenJti)
        {
            try
            {
                var jwt = _citizenIdJwtHelper.CreateAssertedLoginIdentityJwt(idTokenJti);

                return new CreateJwtResult.Success(new CreateJwtResponse { Token = jwt });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception creating JWT");
                return new CreateJwtResult.InternalServerError();
            }
        }
    }
}