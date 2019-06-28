using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public static class VisionLinkagePostErrorMapper
    {
        private static KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>
            KeyAndMessageToError =>
            new KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.InternalCode>()
                .AddKeyToEnum("409V2214",
                    Im1ConnectionErrorCodes.InternalCode.LinkageKeyAlreadyExists)
                .AddKeyToEnum("400V4205",
                    Im1ConnectionErrorCodes.InternalCode.InvalidNhsNumber)
                .AddKeyToEnum("404VY806",
                    Im1ConnectionErrorCodes.InternalCode.PatientRecordNotFound);

        public static LinkageResult Map(VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse> response,
            ILogger<VisionLinkageService> logger)
        {
            var mappedValue = VisionErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue != null
                ? new LinkageResult.ErrorCase(mappedValue.Value)
                : (LinkageResult) new LinkageResult.UnmappedErrorWithStatusCode(response.StatusCode);
        }
    }
}
