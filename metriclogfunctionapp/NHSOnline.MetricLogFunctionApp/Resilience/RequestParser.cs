using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    public sealed class RequestParser<TRequest>
    {
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public RequestParser(ILogger<RequestParser<TRequest>> logger)
        {
            _logger = logger;
            _jsonSerializerSettings = JsonSerializerSettingsCreator.Create();
        }

        internal Task<TRequest> Parse(CloudQueueMessage message)
        {
            _logger.LogEnter();
            try
            {
                var messageText = message.AsString;

                if (string.IsNullOrWhiteSpace(messageText))
                {
                    throw new PermanentException("Cannot process blank message text");
                }

                var request = JsonConvert.DeserializeObject<TRequest>(
                    messageText,
                    _jsonSerializerSettings);

                return Task.FromResult(request);
            }
            catch (JsonException jsonException)
            {
                var errorMessage = "Failed to deserialise ETL request";
                _logger.LogError(jsonException, errorMessage);
                throw new PermanentException(errorMessage, jsonException);
            }
            catch (Exception e)
            {
                _logger.LogMethodFailure(e);
                throw;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }

    public static class  JsonSerializerSettingsCreator
    {
        public static JsonSerializerSettings Create()
        {
            return new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
        }
    }
}