using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageGetErrorMapper : VisionLinkageErrorMapper<LinkageKeyGetResponse>
    {
        protected override Code UnknownError =>
            Im1ConnectionErrorCodes.Code.UnknownError;

        protected override KeyAndMessageToEnumMapper<Code>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Code>()
                .Add("404V2210", "No API key associated with the nhs number",
                    Code.NoApiKeyAssociatedWithNhsNumber)
                .Add("404V2210", "No user associated with the nhs number",
                    Code.NoUserAssociatedWithNhsNumber)
                .AddKeyToEnum("400V4205",
                    Code.InvalidNhsNumber)
                .AddKeyToEnum(
                    "404V4205",
                    Code.InvalidNhsNumber)
                .AddKeyToEnum(
                    "404VY806",
                    Code.PatientRecordNotFound);
    }
}
