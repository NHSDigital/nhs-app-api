using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    internal class TppLinkageErrors
    {
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ILogger _logger;
        private readonly ConfigurationSettings _settings;

        public TppLinkageErrors(IMinimumAgeValidator minimumAgeValidator,
            ILogger logger, IOptions<ConfigurationSettings> settings)
        {
            _minimumAgeValidator = minimumAgeValidator;
            _logger = logger;
            _settings = settings.Value;
        }


        public LinkageResult GetErrorCreatingNhsUser(
            TppClient.TppApiObjectResponse<AddNhsUserResponse> response,
            AddNhsUserRequest originalRequest)
        {
            foreach (var linkageError in KnownErrors())
            {
                if (linkageError.IsInvalid(response, originalRequest))
                {
                    _logger.LogError($"Linkage create request unsuccessful - {linkageError.Message} - {response.ErrorForLogging}");
                    return linkageError.ResultingError;
                }
            }

            _logger.LogError(
                $"Linkage create request unsuccessful - Tpp system is currently unavailable - {response.ErrorForLogging}");
            return new LinkageResult.SupplierSystemUnavailable();
        }

        private List<LinkageError> KnownErrors()
        {
            return new List<LinkageError>
            {
                new LinkageError
                {
                    IsInvalid = (resp, req) => resp.IsUnauthorisedResponse,
                    ResultingError = new LinkageResult.SupplierSystemUnavailable(),
                    Message = "Tpp system is currently unavailable."
                },
                new LinkageError
                {
                    IsInvalid =(resp, req) => resp.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)
                                              && !_minimumAgeValidator.IsValid(req.DateofBirth, _settings.MinimumLinkageAge),
                    ResultingError = new LinkageResult.PatientNonCompetentOrUnderMinimumAge(),
                    Message = "User is under minimum age"
                },
                new LinkageError
                {
                    IsInvalid = (resp, req) => resp.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)
                                               && _minimumAgeValidator.IsValid(req.DateofBirth, _settings.MinimumLinkageAge),
                    ResultingError = new LinkageResult.NotFoundErrorCreatingNhsUser(),
                    Message = "Invalid Linkage Credentials."
                },
                new LinkageError
                {
                    IsInvalid = (resp, req) => resp.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidProviderId),
                    ResultingError = new LinkageResult.InternalServerError(),
                    Message = "Invalid Provider Id."
                },
                new LinkageError
                {
                    IsInvalid =(resp, req) => resp?.ErrorResponse?.ErrorCode != null,
                    Message = "Unknown Error Code",
                    ResultingError = new LinkageResult.InternalServerError()
                }
            };
        }

        private class LinkageError
        {
            public Func<TppClient.TppApiObjectResponse<AddNhsUserResponse>, AddNhsUserRequest, bool> IsInvalid;
            public LinkageResult ResultingError;
            public string Message;
        }
    }
}