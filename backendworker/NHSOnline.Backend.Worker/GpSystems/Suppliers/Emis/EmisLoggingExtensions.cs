using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public static class EmisLoggingExtensions
    {
        public static void LogEmisUnknownError(this ILogger logger, EmisClient.EmisApiResponse response)
        {
            if (IsResponseNull(logger, response)) return;

            var emisMessages = response.ExceptionErrorResponse?.Exceptions?.Select(x => x.Message) ?? new[] { string.Empty };

            var message = string.Join(", ", emisMessages);

            logger.LogError(
                $"Call to EMIS returned an unanticipated error with status code: '{response.StatusCode}'. EMIS message: '{message}'");
        }

        public static void LogEmisResponseIsForbidden(this ILogger logger)
        {
            logger.LogError("Call to EMIS returned a forbidden response");
        }
        
        public static void LogEmisErrorResponse(this ILogger logger, EmisClient.EmisApiResponse response)
        {
            if (IsResponseNull(logger, response)) return;
            try
            {
                var initialResponse = CensorResponse(response);
                if (initialResponse != null)
                {
                    logger.LogError("EMIS Response: " + initialResponse.SerializeJson());
                }
            }
            catch (Exception e)
            {
                logger.LogError("Unable to serialize and log EMIS response");
                logger.LogError(e.StackTrace);
            }
        }

        private static bool IsResponseNull(ILogger logger, EmisClient.EmisApiResponse response)
        {
            if (null != response) return false;
            logger.LogError("Call to EMIS returned a null response");
            return true;
        }

        public static JObject CensorResponse(EmisClient.EmisApiResponse response)
        {
            var initialResponse = JObject.Parse(response.SerializeJson());
            var properties = initialResponse.Descendants()
                .OfType<JProperty>()
                .ToList();

            properties.Where(IsPatientSensitive)
                .ToList()
                .ForEach(attr => attr.Remove());

            properties.Where(ContainsGuid)
                .ToList()
                .ForEach(attr => attr.Value = ReplaceGuid(attr.Value.ToString()));

            return initialResponse;
        }

        private static bool ContainsGuid(JProperty attribute)
        {
            var guidMatch = Regex.Match(attribute.Value.ToString(), Constants.Regex.GuidRegex,
                RegexOptions.IgnoreCase);
            return guidMatch.Success;
        }

        private static string ReplaceGuid(string value)
        {
            var replacedRegex =  Regex.Replace(value,Constants.Regex.GuidRegex,"xxxxxx",
                RegexOptions.IgnoreCase);
            return replacedRegex;
        }

        private static bool IsPatientSensitive(JProperty attribute)
        {
            return PatientFields.PatientSensitiveFields.Contains(attribute.Name);
        }
    }
}