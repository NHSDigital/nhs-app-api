using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public static class VisionLoggingExtensions
    {
        public static void LogVisionErrorResponse<T>(this ILogger logger, T response)
        {
            if (IsResponseNull(logger, response)) return;
            try
            {
                logger.LogError("Vision Error Response: " + response.SerializeJson());
            }
            catch (Exception e)
            {
                logger.LogError("Unable to serialize and log Vision response");
                logger.LogError(e.StackTrace);
            }
        }

        private static bool IsResponseNull<T>(ILogger logger, T response)
        {
            if (null != response) return false;
            logger.LogError("Call to Vision returned a null response");
            return true;
        }
    }
}