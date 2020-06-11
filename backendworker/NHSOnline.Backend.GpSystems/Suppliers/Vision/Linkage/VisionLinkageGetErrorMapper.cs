using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public static class VisionLinkageGetErrorMapper
    {
        private static KeyAndMessageToEnumMapper<InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<InternalCode>()
                .Add("404V2210", "No API key associated with the nhs number",
                    InternalCode.NoApiKeyAssociatedWithNhsNumber)
                .Add("404V2210", "No user associated with the nhs number",
                    InternalCode.NoUserAssociatedWithNhsNumber)
                .AddKeyToEnum("400V4205",
                    InternalCode.InvalidNhsNumber)
                .AddKeyToEnum(
                    "404V4205",
                    InternalCode.InvalidNhsNumber)
                .AddKeyToEnum(
                    "404VY806",
                    InternalCode.PatientRecordNotFound);

        public static LinkageResult Map(VisionLinkageApiObjectResponse<LinkageKeyGetResponse> response,
            ILogger<VisionLinkageService> logger)
        {
            var mappedValue = VisionErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue != null
                ? new LinkageResult.ErrorCase(mappedValue.Value)
                : (LinkageResult)new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
