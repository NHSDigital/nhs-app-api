using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageGetErrorMapper : EmisLinkageErrorMapper<AddVerificationResponse>
    {
        protected override Im1ConnectionErrorCodes.Code UnknownError =>
            Im1ConnectionErrorCodes.Code.UnknownError;

        protected override KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code>()
                .Add("4031030", "Patient Facing Services API v2 is not enabled at this practice",
                    Im1ConnectionErrorCodes.Code.PatientFacingServicesApiv2IsNotEnabledAtThisPractice)
                .Add("4031030", "Patient Facing Services are not enabled by this practice",
                    Im1ConnectionErrorCodes.Code.PatientFacingServicesAreNotEnabledAtThisPractice)
                .AddKeyToEnum(
                    "No registered online user found for given linkage details",
                    Im1ConnectionErrorCodes.Code.NoSelfAssociatedUserExistWithThisPatient)
                .AddKeyToEnum(
                    "4001553",
                    Im1ConnectionErrorCodes.Code.UnderMinimumAgeOrNonCompetent)
                .AddKeyToEnum(
                    "4001555",
                    Im1ConnectionErrorCodes.Code.MultipleRecordsFoundWithNhsNumber)
                .AddKeyToEnum(
                    "4001554",
                    Im1ConnectionErrorCodes.Code.NotValidForOnlineUser)
                .AddKeyToEnum(
                    "4001001",
                    Im1ConnectionErrorCodes.Code.UnableToFindOrganisation)
                .AddKeyToEnum(
                    "4001107",
                    Im1ConnectionErrorCodes.Code.UserSelfAssociatedAccountIsArchived)
                .AddKeyToEnum(
                    "4001552",
                    Im1ConnectionErrorCodes.Code.PatientArchived)
                .AddKeyToEnum(
                    "4001109",
                    Im1ConnectionErrorCodes.Code.UserSelfAssociatedAccountNotLinkedWithPatient)
                .AddKeyToEnum(
                    "4001401",
                    Im1ConnectionErrorCodes.Code.PracticeNotLive)
                .AddKeyToEnum(
                    "4041104",
                    Im1ConnectionErrorCodes.Code.NoSelfAssociatedUserExistWithThisPatient)
                .AddKeyToEnum(
                    "4041551",
                    Im1ConnectionErrorCodes.Code.PatientNotRegisteredAtThisPractice);
    }
}
