using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public static class EmisLinkageGetErrorMapper 
    {
        private static KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>()
                .Add("4031030", "Patient Facing Services API v2 is not enabled at this practice",
                    Im1ConnectionErrorCodes.InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)
                .Add("4031030", "Patient Facing Services are not enabled by this practice",
                    Im1ConnectionErrorCodes.InternalCode.PatientFacingServicesAreNotEnabledAtThisPractice)
                .AddMessageToEnum(
                    "No registered online user found for given linkage details",
                    Im1ConnectionErrorCodes.InternalCode.NoSelfAssociatedUserExistWithThisPatient)
                .AddKeyToEnum(
                    "4001553",
                    Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent)
                .AddKeyToEnum(
                    "4001555",
                    Im1ConnectionErrorCodes.InternalCode.MultipleRecordsFoundWithNhsNumber)
                .AddKeyToEnum(
                    "4001554",
                    Im1ConnectionErrorCodes.InternalCode.NotValidForOnlineUser)
                .AddKeyToEnum(
                    "4001001",
                    Im1ConnectionErrorCodes.InternalCode.UnableToFindOrganisation)
                .AddKeyToEnum(
                    "4001107",
                    Im1ConnectionErrorCodes.InternalCode.UserSelfAssociatedAccountIsArchived)
                .AddKeyToEnum(
                    "4001552",
                    Im1ConnectionErrorCodes.InternalCode.PatientArchived)
                .AddKeyToEnum(
                    "4001109",
                    Im1ConnectionErrorCodes.InternalCode.UserSelfAssociatedAccountNotLinkedWithPatient)
                .AddKeyToEnum(
                    "4001401",
                    Im1ConnectionErrorCodes.InternalCode.PracticeNotLive)
                .AddKeyToEnum(
                    "4041104",
                    Im1ConnectionErrorCodes.InternalCode.NoSelfAssociatedUserExistWithThisPatient)
                .AddKeyToEnum(
                    "4041551",
                    Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtThisPractice);


        public static LinkageResult Map(EmisClient.EmisApiObjectResponse<AddVerificationResponse> response,
            ILogger<EmisLinkageService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue != null ? new LinkageResult.ErrorCase(mappedValue.Value) :
                (LinkageResult)new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
