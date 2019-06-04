using System.Collections.Generic;
using System.ComponentModel;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public interface IIm1ConnectionErrorCodes
    {
        ApiErrorResponse GetErrorResponse(Im1ConnectionErrorCodes.Code code);
    }

#pragma warning disable CA1008 // Enums should have zero value
    public class Im1ConnectionErrorCodes : IIm1ConnectionErrorCodes
    {
        public ApiErrorResponse GetErrorResponse(Code code)
        {
            return ErrorResponses[(int)code];
        }

        public Dictionary<int, ApiErrorResponse> ErrorResponses { get; } = CreateErrors();

        private static Dictionary<int, ApiErrorResponse> CreateErrors()
        {
            return new ErrorCodes<Code>().GetAllErrorResponses();
        }

        //Any changes to this enum must also be made in the swagger contract
        public enum Code
        {
            [Description("Unknown Error")]
            UnknownError = 100,

            [Description("Invalid linkage details")]
            InvalidLinkageDetails = 101,
            [Description("No match found for given demographics")]
            NoMatchFoundForGivenDemographics = 102,
            [Description("Unable to find organisation for NationalPracticeCode")]
            UnableToFindOrganisation = 103,
            [Description("User account is inactive or archived, legacy linking not allowed")]
            UserAccountIsInactiveOrArchived = 104,
            [Description("Practice not live")]
            PracticeNotLive = 105,
            [Description("Invalid Nhs Number")]
            InvalidNhsNumber = 106,
            [Description("Invalid linkage details")]
            InvalidLinkageDetailsTpp = 107,
            [Description("Patient Facing Services API v2 is not enabled at this practice")]
            PatientFacingServicesApiv2IsNotEnabledAtThisPractice = 108,
            [Description("Patient Facing Services are not enabled by this practice")]
            PatientFacingServicesAreNotEnabledAtThisPractice = 109,
            [Description("No user found for linkage details")]
            NoUserFoundForLinkageDetails = 110,
            [Description("User is already linked")]
            UserAlreadyLinked = 111,
            [Description("Patient marked as archived")]
            PatientArchived = 112,
            [Description("User account disabled")]
            UserAccountDisabled = 113,
            [Description("Under minimum age or non competant")]
            UnderMinimumAgeOrNonCompetent = 114,
            [Description("Patient status not valid for online user account process")]
            NotValidForOnlineUser = 115,
            [Description("Multiple Records Found With Nhs Number")]
            MultipleRecordsFoundWithNhsNumber = 116,
            [Description("User self associated account is archived")]
            UserSelfAssociatedAccountIsArchived = 117,
            [Description("Invalid Provider Id")]
            InvalidProviderId = 118,
            [Description("There was a problem linking your account")]
            ProblemLinkingAccount = 119,
            [Description("User account already exists with patient demographic details")]
            UserAccountAlreadyExistsWithPatientDemographicDetails = 120,
            [Description("User account already exists with patient demographic details and has been archived")]
            UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived = 121,
            [Description("Linkage key already exists")]
            LinkageKeyAlreadyExists = 122,
            [Description("User self-associated account is not linked with patient")]
            UserSelfAssociatedAccountNotLinkedWithPatient = 123,
            [Description("Connection to service failed")]
            ConnectionToServiceFailed = 124,
            [Description("Length of account id is outside valid range")]
            AccountIdLengthOutsideOfValidRange = 125,
            [Description("Length of linkage key is outside valid range")]
            LinkageKeyLengthOutsideOfValidRange = 126,
            [Description("Unidentified value outside valid range")]
            InputValueLengthOutsideOfValidRange = 127,

            [Description("Patient not registered at this practice")]
            PatientNotRegisteredAtThisPractice = 195,
            [Description("No API key associated with the nhs number")]
            NoApiKeyAssociatedWithNhsNumber = 196,
            [Description("No user associated with the nhs number")]
            NoUserAssociatedWithNhsNumber = 197,
            [Description("Patient record not found")]
            PatientRecordNotFound = 198,
            [Description("No self-associated user exist with this patient")]
            NoSelfAssociatedUserExistWithThisPatient = 199
        }
    }
}
