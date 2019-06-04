using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkagePostErrorMapper : EmisLinkageErrorMapper<AddNhsUserResponse>
    {
        protected override Im1ConnectionErrorCodes.Code UnknownError =>
            Im1ConnectionErrorCodes.Code.UnknownError;

        protected override KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>()
                .AddKeyToEnum(
                    "4001553",
                    Im1ConnectionErrorCodes.Code.UnderMinimumAgeOrNonCompetent)
                .AddKeyToEnum(
                    "4091110",
                    Im1ConnectionErrorCodes.Code.UserAccountAlreadyExistsWithPatientDemographicDetails)
                .AddKeyToEnum(
                    "4001107",
                    Im1ConnectionErrorCodes.Code.UserAccountAlreadyExistsWithPatientDemographicDetailsAndIsArchived)
                 .AddKeyToEnum(
                    "4041551",
                    Im1ConnectionErrorCodes.Code.PatientNotRegisteredAtThisPractice)
                .AddKeyToEnum(
                    "4001552",
                    Im1ConnectionErrorCodes.Code.PatientArchived);

    }
}
