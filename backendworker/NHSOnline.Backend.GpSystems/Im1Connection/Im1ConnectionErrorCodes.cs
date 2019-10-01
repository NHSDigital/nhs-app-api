using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public interface IIm1ConnectionErrorCodes
    {
        Im1ErrorResponse GetAndLogErrorResponse(Im1ConnectionErrorCodes.InternalCode internalCode, Supplier supplier, ILogger logger);
        Im1ConnectionErrorCodes.ExternalCode GetExternalCode(Im1ConnectionErrorCodes.InternalCode internalCode);
    }

    public class Im1ConnectionErrorCodes : IIm1ConnectionErrorCodes
    {
        public Im1ErrorResponse GetAndLogErrorResponse(InternalCode internalCode, Supplier supplier, ILogger logger)
        {
            var internalErrorMessage = InternalErrorResponses[(int)internalCode].ErrorMessage;
            var externalCode = GetExternalCode(internalCode);
            var externalError = ExternalErrorResponses[(int) externalCode];
            var externalErrorMessage = externalError.ErrorMessage;
            logger.LogInformation($"Linkage Error. Internal Error code: {internalCode} : {(int)internalCode} : {internalErrorMessage}" +
                                  $"\nMapped to External Error code: {externalCode} : {(int)externalCode} : {externalErrorMessage}");
            externalError.GpSystem = supplier.ToString();
            return externalError;
        }

        public ExternalCode GetExternalCode(InternalCode internalCode)
        {
            if (internalCode == InternalCode.InvalidOption)
            {
                throw new ArgumentException($"{InternalCode.InvalidOption} is not a valid option");
            }

            return ErrorCodeToGenericErrorCode.GetValueOrDefault(internalCode);
        }

        public Dictionary<int, Im1ErrorResponse> ExternalErrorResponses { get; } = new ErrorCodes<ExternalCode, Im1ErrorResponse>().GetAllErrorResponses();
        public Dictionary<int, Im1ErrorResponse> InternalErrorResponses { get; } = new ErrorCodes<InternalCode, Im1ErrorResponse>().GetAllErrorResponses();
        
        public enum InternalCode
        {
            [Description("Unknown Error")]
            InvalidOption = 0,
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
            [Description("Problem logging in. Please retry")]
            ProblemLoggingIn = 128,
            [Description("Record currently unavailable - please try again later or contact your Practice")]
            UserRecordUnavailable = 129,
            [Description("Invalid security")]
            InvalidSecurity = 130,
            [Description("Invalid request, please try again")]
            InvalidRequest = 131,
            [Description("Invalid user patient link token, please try again")]
            InvalidUserPatientLinkToken = 132,
            [Description("Patient not registered at this practice")]
            PatientNotRegisteredAtThisPractice = 195,
            [Description("No API key associated with the nhs number")]
            NoApiKeyAssociatedWithNhsNumber = 196,
            [Description("No user associated with the nhs number")]
            NoUserAssociatedWithNhsNumber = 197,
            [Description("Patient record not found")]
            PatientRecordNotFound = 198,
            [Description("No self-associated user exist with this patient")]
            NoSelfAssociatedUserExistWithThisPatient = 199,
            [Description("Linkage keys are currently not supported by the supplier")]
            LinkageKeysNotSupportedBySupplier = 550
        }

        //Any changes to this enum must also be made in the swagger contract
        public enum ExternalCode
        {
            [Description("Unknown Error")]
            InvalidOption = 0,
            [Description("Unknown Error")]
            UnknownError = 100,
            [Description("Practice not enabled")]
            PracticeNotEnabled = 101,
            [Description("Patient archived or disabled")]
            PatientArchivedOrDisabled = 102,
            [Description("Patient not found")]
            PatientNotFound = 103,
            [Description("Under minimum age or non competant")]
            UnderMinimumAgeOrNonCompetent = 104,
            [Description("User account conflict")]
            UserAccountConflict = 105,
            [Description("Invalid Details")]
            InvalidDetails = 106,
            [Description("Upstream Error")]
            UpstreamError = 107,
            [Description("Invalid Nhs Number")]
            InvalidNhsNumber = 108,
            [Description("Invalid Provider Id")]
            InvalidProviderId = 109,
            [Description("Invalid Account Id")]
            InvalidAccountId = 110,
            [Description("Invalid Linkage Key")]
            InvalidLinkageKey = 111,
            [Description("Problem logging in, please retry")]
            ProblemLoggingIn = 112,
            [Description("Record currently unavailable, please retry")]
            UserRecordUnavailable = 113,
            [Description("Invalid Security")]
            InvalidSecurity = 114,
            [Description("Invalid request, please try again")]
            InvalidRequest = 115,
            [Description("Invalid user patient link token")]
            InvalidUserPatientLinkToken = 116,
            [Description("Linkage not found, prompt to create new linkage key")]
            LinkageNotFound = 199,
        }

        private static readonly Dictionary<InternalCode, ExternalCode> ErrorCodeToGenericErrorCode =
            new Dictionary<InternalCode, ExternalCode>
            {
                { InternalCode.InvalidOption, ExternalCode.InvalidOption },
                { InternalCode.UserAccountIsInactiveOrArchived, ExternalCode.PatientArchivedOrDisabled },
                { InternalCode.PatientArchived,ExternalCode.PatientArchivedOrDisabled },
                { InternalCode.UserAccountDisabled,ExternalCode.PatientArchivedOrDisabled },
                { InternalCode.UserSelfAssociatedAccountIsArchived, ExternalCode.PatientArchivedOrDisabled },
                { InternalCode.UserSelfAssociatedAccountNotLinkedWithPatient, ExternalCode.PatientArchivedOrDisabled },
                {
                    InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived,
                    ExternalCode.PatientArchivedOrDisabled
                },
                { InternalCode.NotValidForOnlineUser, ExternalCode.PatientArchivedOrDisabled },

                { InternalCode.PracticeNotLive, ExternalCode.PracticeNotEnabled },
                { InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice,ExternalCode.PracticeNotEnabled  },
                { InternalCode.PatientFacingServicesAreNotEnabledAtThisPractice, ExternalCode.PracticeNotEnabled },
                { InternalCode.UnableToFindOrganisation, ExternalCode.PracticeNotEnabled },

                { InternalCode.UnderMinimumAgeOrNonCompetent, ExternalCode.UnderMinimumAgeOrNonCompetent },

                { InternalCode.NoMatchFoundForGivenDemographics,ExternalCode.PatientNotFound  },
                { InternalCode.NoUserFoundForLinkageDetails, ExternalCode.PatientNotFound },

                { InternalCode.InvalidLinkageDetails,ExternalCode.InvalidDetails },
                { InternalCode.InvalidLinkageDetailsTpp, ExternalCode.InvalidDetails },
                { InternalCode.InvalidNhsNumber, ExternalCode.InvalidNhsNumber },
                { InternalCode.AccountIdLengthOutsideOfValidRange, ExternalCode.InvalidAccountId },
                { InternalCode.LinkageKeyLengthOutsideOfValidRange, ExternalCode.InvalidLinkageKey},
                { InternalCode.InputValueLengthOutsideOfValidRange, ExternalCode.InvalidDetails},
                { InternalCode.InvalidProviderId,  ExternalCode.InvalidProviderId},

                { InternalCode.UserAlreadyLinked, ExternalCode.UserAccountConflict },
                { InternalCode.MultipleRecordsFoundWithNhsNumber, ExternalCode.UserAccountConflict  },
                {
                  InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetails,
                  ExternalCode.UserAccountConflict
                },
                { InternalCode.LinkageKeyAlreadyExists,  ExternalCode.UserAccountConflict },

                { InternalCode.ConnectionToServiceFailed, ExternalCode.UpstreamError },

                { InternalCode.UnknownError, ExternalCode.UnknownError },
                { InternalCode.ProblemLinkingAccount, ExternalCode.UnknownError },
                { InternalCode.ProblemLoggingIn, ExternalCode.ProblemLoggingIn},
                { InternalCode.UserRecordUnavailable, ExternalCode.UserRecordUnavailable },
                { InternalCode.InvalidSecurity, ExternalCode.InvalidSecurity },
                { InternalCode.InvalidRequest, ExternalCode.InvalidRequest },
                { InternalCode.InvalidUserPatientLinkToken, ExternalCode.InvalidUserPatientLinkToken },

                // The following error codes are caught earlier in the process.
                // They are included here for completeness, but should never reach this stage.
                // (Failure to get linkage key prompts a create).
                { InternalCode.PatientNotRegisteredAtThisPractice, ExternalCode.LinkageNotFound },
                { InternalCode.NoApiKeyAssociatedWithNhsNumber,  ExternalCode.LinkageNotFound },
                { InternalCode.NoUserAssociatedWithNhsNumber,  ExternalCode.LinkageNotFound },
                { InternalCode.PatientRecordNotFound, ExternalCode.LinkageNotFound },
                { InternalCode.NoSelfAssociatedUserExistWithThisPatient, ExternalCode.LinkageNotFound }
            };
    }
}
