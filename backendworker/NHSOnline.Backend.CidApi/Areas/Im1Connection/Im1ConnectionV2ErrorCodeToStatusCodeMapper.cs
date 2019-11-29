using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public static class Im1ConnectionV2ErrorCodeToStatusCodeMapper
    {
        private static readonly Dictionary<Im1ConnectionErrorCodes.ExternalCode, int> ErrorCodeToStatusCode =
            new Dictionary<Im1ConnectionErrorCodes.ExternalCode, int>
            {
                { Im1ConnectionErrorCodes.ExternalCode.InvalidOption, StatusCodes.Status500InternalServerError },
                { Im1ConnectionErrorCodes.ExternalCode.UnknownError, StatusCodes.Status500InternalServerError },
                { Im1ConnectionErrorCodes.ExternalCode.PracticeNotEnabled, StatusCodes.Status403Forbidden },
                { Im1ConnectionErrorCodes.ExternalCode.PatientArchivedOrDisabled, StatusCodes.Status403Forbidden },
                { Im1ConnectionErrorCodes.ExternalCode.PatientNotFound, StatusCodes.Status404NotFound },
                { Im1ConnectionErrorCodes.ExternalCode.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden },
                { Im1ConnectionErrorCodes.ExternalCode.UserAccountConflict, StatusCodes.Status409Conflict},
                { Im1ConnectionErrorCodes.ExternalCode.InvalidDetails, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.UpstreamError, StatusCodes.Status502BadGateway },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidNhsNumber, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidProviderId, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidAccountId, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidLinkageKey, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.ProblemLoggingIn, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.UserRecordUnavailable, StatusCodes.Status502BadGateway },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidSecurity, StatusCodes.Status400BadRequest },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidRequest, StatusCodes.Status502BadGateway },
                { Im1ConnectionErrorCodes.ExternalCode.InvalidUserPatientLinkToken, StatusCodes.Status400BadRequest },
                // LinkageNotFound prompts a linkage key to be created
                // It is included here for completeness, but should never reach this stage.
                { Im1ConnectionErrorCodes.ExternalCode.LinkageNotFound, StatusCodes.Status500InternalServerError },
                { Im1ConnectionErrorCodes.ExternalCode.LinkageKeysNotSupportedBySupplier, StatusCodes.Status400BadRequest }
            };

        public static int Map(Im1ConnectionErrorCodes.ExternalCode errorCode)
        {
            return ErrorCodeToStatusCode.GetValueOrDefault(errorCode);
        }
    }
}