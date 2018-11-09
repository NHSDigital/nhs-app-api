using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    internal class TppLinkageErrors
    {
        public static LinkageResult GetErrorCreatingNhsUser(ILogger logger, TppClient.TppApiObjectResponse<AddNhsUserResponse> response, AddNhsUserRequest originalRequest)
        {
            foreach (var linkageError in KnownLinkageErrors)
            {
                if (linkageError.IsInvalid(response, originalRequest))
                {
                    logger.LogError($"Linkage create request unsuccessful - {linkageError.Message} - {response.ErrorForLogging()}");
                    return linkageError.ResultingError;
                }
            }

            logger.LogError(
                $"Linkage create request unsuccessful - Tpp system is currently unavailable - {response.ErrorForLogging()}");
            return new LinkageResult.SupplierSystemUnavailable();
        }


        private static readonly IList<LinkageError> KnownLinkageErrors = new List<LinkageError>
        {
            new LinkageError
            {
                IsInvalid = (resp, req) => resp.NotAuthenticated,
                ResultingError = new LinkageResult.SupplierSystemUnavailable(),
                Message = "Tpp system is currently unavailable."
            },
            new LinkageError
            {
                IsInvalid =(resp, req) => resp.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)
                                          && IsUnderSixteen(req),
                ResultingError = new LinkageResult.PatientNonCompetentOrUnder16(),
                Message = "User is under 16"
            },
            new LinkageError
            {
                IsInvalid = (resp, req) => resp.HasErrorWithCode(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)
                                           && !IsUnderSixteen(req),
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

        private static bool IsUnderSixteen(AddNhsUserRequest request)
        {
            var age = CalculateAge(request.DateofBirth);
            return age < 16;
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            var years = now.Year - dateOfBirth.Year;

            if (dateOfBirth.Month > now.Month || dateOfBirth.Month == now.Month && dateOfBirth.Day > now.Day)
                years--;

            return years;
        }

        private class LinkageError
        {
            public Func<TppClient.TppApiObjectResponse<AddNhsUserResponse>, AddNhsUserRequest, bool> IsInvalid;
            public LinkageResult ResultingError;
            public string Message;
        }

    }
}