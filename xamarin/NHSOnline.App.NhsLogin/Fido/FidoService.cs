using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class FidoService : IFidoService
    {
        private readonly ILogger _logger;
        private readonly FidoRegistrationService _registrationService;
        private readonly FidoAuthorisationService _authorisationService;

        private static readonly List<string> UnrecoverableAndroidKeyStoreExceptions = new List<string>
        {
            // This exception wrapper covers a multitude of android.security.KeyStoreExceptions which are raised by 'KeyStore2.java' class.
            "java.security.UnrecoverableKeyException",
            // The following KeyStoreExceptions do not get wrapped in the Java one above, but we will handle in the same way.
            "android.security.KeyStoreException: Key user not authenticated"
        };

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
            catch (Exception e) when (IsUnrecoverableAndroidKeyStoreException(e))
            {
                _logger.LogError(e, "Unrecoverable Android KeyStore Exception detected");
                return new FidoAuthorisationResult.PermanentLockout();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "FIDO authorise unsuccessful");
                return new FidoAuthorisationResult.Unauthorised();
            }
        }

        private static bool IsUnrecoverableAndroidKeyStoreException(Exception exception) =>
            UnrecoverableAndroidKeyStoreExceptions.Any(knownKeyStoreExceptionStringToMatch =>
                exception.ToString().Contains(knownKeyStoreExceptionStringToMatch, StringComparison.Ordinal));
    }
}