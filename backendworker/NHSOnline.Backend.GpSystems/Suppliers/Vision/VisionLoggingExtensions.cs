using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public static class VisionLoggingExtensions
    {
        public static void LogVisionErrorResponse<T>(this ILogger logger, VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            if (response == null)
            {
                logger.LogError("Vision Error Response: Null");
                return;
            }

            try
            {
                if (response.RawResponse != null)
                {
                    var censoredResponse = CensorResponse(response);
                    logger.LogError("Vision Error Response: " + censoredResponse.SerializeJson());
                }
                else
                {
                    logger.LogError("Vision Error Response: " + response.UnparsableResultMessage);
                }
            }
            catch (Exception e)
            {
                logger.LogError("Unable to serialize and log Vision response");
                logger.LogError(e.StackTrace);
            }
        }

        public static JObject CensorResponse<T>(VisionPFSClient.VisionApiObjectResponse<T> response)
        {
            var initialResponse = JObject.Parse(response.RawResponse.SerializeJson());
            var properties = initialResponse.Descendants()
                .OfType<JProperty>()
                .ToList();

            properties.ForEach(ReplaceGuid);

            return initialResponse;
        }

        private static void ReplaceGuid(JProperty attribute)
        {
            attribute.Value = Regex.Replace(attribute.Value.ToString(), Constants.Regex.GuidRegex, "xxxxxx",
                RegexOptions.IgnoreCase);
        }
    }
}
