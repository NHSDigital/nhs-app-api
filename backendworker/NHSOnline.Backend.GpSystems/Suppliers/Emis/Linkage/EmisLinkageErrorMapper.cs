using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public abstract class EmisLinkageErrorMapper<TResponse> : LinkageErrorMapper
    {
        protected abstract KeyAndMessageToEnumMapper<Im1ConnectionErrorCodes.Code> KeyAndMessageToError { get; }

        public LinkageResult Map(EmisClient.EmisApiObjectResponse<TResponse> response,
            ILogger<EmisLinkageService> logger)
        {
            var mappedValue = EmisErrorMapper.Map(logger, response, KeyAndMessageToError);
            return mappedValue !=null ? new LinkageResult.ErrorCase(mappedValue.Value) : MapUnknownError(response.StatusCode);
        }
    }
}