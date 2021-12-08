using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class FidoService : IFidoService
    {
        private readonly ILogger _logger;
        private readonly FidoRegistrationService _registrationService;
        private readonly FidoAuthorisationService _authorisationService;

        public FidoService(
            ILogger<FidoService> logger,
            FidoRegistrationService registrationService,
            FidoAuthorisationService authorisationService)
        {
            _logger = logger;
            _registrationService = registrationService;
            _authorisationService = authorisationService;
        }

        public async Task<FidoRegisterResult> Register(IFidoKey key, string accessToken)
        {
            try
            {
                return await _registrationService.Register(key, accessToken).ResumeOnThreadPool();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "FIDO registration failed");
                return new FidoRegisterResult.Failed();
            }
        }

        public async Task Deregister(string accessToken, string keyId)
        {
            try
            {
                await _registrationService.Deregister(accessToken, keyId).ResumeOnThreadPool();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "FIDO deregister failed");
            }
        }

        public async Task<FidoAuthorisationResult> Authorise(IFidoKey fidoKey)
        {
            try
            {
                return await _authorisationService.Authorise(fidoKey).ResumeOnThreadPool();
            }
            catch (CrossPlatformException e) when (e.ErrorType is CrossPlatformErrorType.UnrecoverableKey)
            {
                _logger.LogError(e, "Unrecoverable Exception detected");
                return new FidoAuthorisationResult.PermanentLockout();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "FIDO authorise unsuccessful");
                return new FidoAuthorisationResult.Unauthorised();
            }
        }
    }
}