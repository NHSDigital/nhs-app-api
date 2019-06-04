using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal static class Im1ConnectionV2ErrorCodeMapper
    {
        private static readonly Dictionary<Code, int> ErrorCodeToStatusCode =
            new Dictionary<Code, int>
            {
                { Code.InvalidLinkageDetails, StatusCodes.Status400BadRequest },
                { Code.InvalidLinkageDetailsTpp, StatusCodes.Status400BadRequest },
                { Code.NoMatchFoundForGivenDemographics, StatusCodes.Status404NotFound },
                { Code.UnableToFindOrganisation, StatusCodes.Status400BadRequest },
                { Code.UserAccountIsInactiveOrArchived, StatusCodes.Status403Forbidden },
                { Code.PracticeNotLive, StatusCodes.Status400BadRequest },
                { Code.InvalidNhsNumber, StatusCodes.Status400BadRequest },
                { Code.AccountIdLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { Code.LinkageKeyLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { Code.InputValueLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { Code.PatientFacingServicesApiv2IsNotEnabledAtThisPractice, StatusCodes.Status400BadRequest },
                { Code.PatientFacingServicesAreNotEnabledAtThisPractice, StatusCodes.Status400BadRequest },
                { Code.PatientArchived, StatusCodes.Status403Forbidden },
                { Code.NoUserFoundForLinkageDetails, StatusCodes.Status404NotFound },
                { Code.UserAlreadyLinked, StatusCodes.Status409Conflict },
                { Code.ConnectionToServiceFailed, StatusCodes.Status400BadRequest },
                { Code.UserAccountDisabled, StatusCodes.Status403Forbidden },
                { Code.UnknownError, StatusCodes.Status400BadRequest },
                { Code.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden },
                { Code.MultipleRecordsFoundWithNhsNumber, StatusCodes.Status400BadRequest },
                { Code.NotValidForOnlineUser, StatusCodes.Status400BadRequest },
                { Code.UserSelfAssociatedAccountIsArchived, StatusCodes.Status403Forbidden },
                { Code.UserSelfAssociatedAccountNotLinkedWithPatient, StatusCodes.Status400BadRequest },
                { Code.InvalidProviderId, StatusCodes.Status403Forbidden },
                { Code.ProblemLinkingAccount, StatusCodes.Status404NotFound },
                {
                    Code.UserAccountAlreadyExistsWithPatientDemographicDetails,
                    StatusCodes.Status400BadRequest
                },
                {
                    Code.UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived,
                    StatusCodes.Status403Forbidden
                },
                { Code.LinkageKeyAlreadyExists, StatusCodes.Status409Conflict },

                // The following error codes are caught earlier in the process.
                // They are included here for completeness, but should never reach this stage.
                // (Failure to get linkage key prompts a create).
                { Code.PatientNotRegisteredAtThisPractice, StatusCodes.Status500InternalServerError },
                { Code.NoApiKeyAssociatedWithNhsNumber, StatusCodes.Status500InternalServerError },
                { Code.NoUserAssociatedWithNhsNumber, StatusCodes.Status500InternalServerError },
                { Code.PatientRecordNotFound, StatusCodes.Status500InternalServerError },
                { Code.NoSelfAssociatedUserExistWithThisPatient, StatusCodes.Status500InternalServerError }
            };

        public static int Map(Code errorCode)
        {
            return ErrorCodeToStatusCode.GetValueOrDefault(errorCode);
        }
    }
}