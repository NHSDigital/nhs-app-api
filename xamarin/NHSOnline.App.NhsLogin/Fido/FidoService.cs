using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class FidoService: IFidoService
    {
        private readonly ILogger _logger;
        private readonly FidoRegistrationService _registrationService;

        public FidoService(
            ILogger<FidoService> logger,
            FidoRegistrationService registrationService)
        {
            _logger = logger;
            _registrationService = registrationService;
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
    }
}
