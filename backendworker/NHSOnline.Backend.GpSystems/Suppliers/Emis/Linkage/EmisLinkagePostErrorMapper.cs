using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public static class EmisLinkagePostErrorMapper
    {
        private static KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>()
                .AddKeyToEnum(
                    "4001553",
                    Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent)
                .AddKeyToEnum(
                    "4001555",
                    Im1ConnectionErrorCodes.InternalCode.MultipleRecordsFoundWithNhsNumber)
                .AddKeyToEnum(
                    "4091110",
                    Im1ConnectionErrorCodes.InternalCode.UserAccountAlreadyExistsWithPatientDemographicDetails)
                .AddKeyToEnum(
                    "4001107",
                    Im1ConnectionErrorCodes.InternalCode
                        .UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived)
                .AddKeyToEnum(
                    "4041551",
                    Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtThisPractice)
                .AddKeyToEnum(
                    "4001401",
                    Im1ConnectionErrorCodes.InternalCode.PracticeNotLive)
                .AddKeyToEnum(
                    "4001552",
                    Im1ConnectionErrorCodes.InternalCode.PatientArchived)
                .AddKeyToEnum(
                    "4090",
                    Im1ConnectionErrorCodes.InternalCode.UserAlreadyLinked);

        public static LinkageResult Map(EmisApiObjectResponse<AddNhsUserResponse> response,
            ILogger<EmisLinkageService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue != null
                ? new LinkageResult.ErrorCase(mappedValue.Value)
                : (LinkageResult) new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
