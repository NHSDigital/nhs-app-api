using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Logging
{
    internal sealed class CloudLoggingService : ICloudLog, ICreateLogResultVisitor<ApiCreateLogResult>
    {
        private readonly IApiClientEndpoint<ApiCreateLogRequest, ApiCreateLogResult> _createLogEndpoint;

        public CloudLoggingService(
            IApiClientEndpoint<ApiCreateLogRequest, ApiCreateLogResult> createLogEndpoint)
        {
            _createLogEndpoint = createLogEndpoint;
        }

        public async Task Log(LogLevel logLevel, string context, string message, DateTime timeStamp)
        {
            try
            {
                var request = new ApiCreateLogRequest(message, logLevel, timeStamp);
                await _createLogEndpoint.Call(request).ResumeOnThreadPool();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to log: {0}", e);
            }
        }

        public ApiCreateLogResult Visit(ApiCreateLogResult.Created created)
        {
            return new ApiCreateLogResult.Created();
        }

        public ApiCreateLogResult Visit(ApiCreateLogResult.Failure failed)
        {
            return new ApiCreateLogResult.Failure();
        }
    }
}