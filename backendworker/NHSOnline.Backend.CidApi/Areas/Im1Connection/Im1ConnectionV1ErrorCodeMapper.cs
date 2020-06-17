using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal static class Im1ConnectionV1ErrorCodeMapper
    {
        private const int Status550LinkageNotSupported = 550;

        private static readonly Dictionary<InternalCode, int> ErrorCodeToStatusCode =
            new Dictionary<InternalCode, int>
            {
                { InternalCode.InvalidOption, StatusCodes.Status500InternalServerError },
                { InternalCode.InvalidLinkageDetails, StatusCodes.Status400BadRequest },
                { InternalCode.NoMatchFoundForGivenDemographics, StatusCodes.Status404NotFound },
                { InternalCode.ErrorRetrievingGivenDemographics, StatusCodes.Status403Forbidden },
                { InternalCode.UnableToFindOrganisation, StatusCodes.Status400BadRequest },
                { InternalCode.UserAccountIsInactiveOrArchived, StatusCodes.Status403Forbidden },
                { InternalCode.PracticeNotLive, StatusCodes.Status400BadRequest },
                { InternalCode.InvalidNhsNumber, StatusCodes.Status400BadRequest },
                { InternalCode.AccountIdLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { InternalCode.LinkageKeyLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { InternalCode.InputValueLengthOutsideOfValidRange, StatusCodes.Status400BadRequest },
                { InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice, StatusCodes.Status400BadRequest },
                { InternalCode.PatientFacingServicesAreNotEnabledAtThisPractice, StatusCodes.Status400BadRequest },
                { InternalCode.PatientArchived, StatusCodes.Status403Forbidden },
                { InternalCode.NoUserFoundForLinkageDetails, StatusCodes.Status404NotFound },
                { InternalCode.UserAlreadyLinked, StatusCodes.Status409Conflict },
                { InternalCode.ConnectionToServiceFailed, StatusCodes.Status502BadGateway },
                { InternalCode.UserAccountDisabled, StatusCodes.Status403Forbidden },
                { InternalCode.UnknownError, StatusCodes.Status400BadRequest },
                { InternalCode.InvalidLinkageDetailsTpp, StatusCodes.Status404NotFound },
                { InternalCode.ProblemLoggingIn, StatusCodes.Status404NotFound },
                { InternalCode.UnderMinimumAgeOrNonCompetent, StatusCodes.Status403Forbidden },
                { InternalCode.MultipleRecordsFoundWithNhsNumber, StatusCodes.Status403Forbidden },
                { InternalCode.NotValidForOnlineUser, StatusCodes.Status400BadRequest },
                { InternalCode.UserSelfAssociatedAccountIsArchived, StatusCodes.Status403Forbidden },
                { InternalCode.UserSelfAssociatedAccountNotLinkedWithPatient, StatusCodes.Status400BadRequest },
                { InternalCode.PatientNotRegisteredAtThisPractice, StatusCodes.Status404NotFound },
                { InternalCode.NoSelfAssociatedUserExistWithThisPatient, StatusCodes.Status404NotFound },
                { InternalCode.NoApiKeyAssociatedWithNhsNumber, StatusCodes.Status404NotFound },
                { InternalCode.NoUserAssociatedWithNhsNumber, StatusCodes.Status404NotFound },
                { InternalCode.PatientRecordNotFound, StatusCodes.Status404NotFound },
                { InternalCode.InvalidProviderId, StatusCodes.Status403Forbidden },
                { InternalCode.ProblemLinkingAccount, StatusCodes.Status404NotFound },
                { InternalCode.UserRecordUnavailable, StatusCodes.Status502BadGateway },
                { InternalCode.InvalidSecurity, StatusCodes.Status500InternalServerError },
                { InternalCode.InvalidRequest, StatusCodes.Status400BadRequest },
                { InternalCode.InvalidUserPatientLinkToken, StatusCodes.Status400BadRequest },
                {
                    InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetails,
                    StatusCodes.Status400BadRequest
                },
                {
                    InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived,
                    StatusCodes.Status403Forbidden
                },
                { InternalCode.RegistrationIncomplete, StatusCodes.Status403Forbidden },
                { InternalCode.LinkageKeyAlreadyExists, StatusCodes.Status409Conflict },
                { InternalCode.LinkageKeysNotSupportedBySupplier, Status550LinkageNotSupported }
            };

        public static int Map(InternalCode errorCode)
        {
            return ErrorCodeToStatusCode.GetValueOrDefault(errorCode);
        }
    }
}