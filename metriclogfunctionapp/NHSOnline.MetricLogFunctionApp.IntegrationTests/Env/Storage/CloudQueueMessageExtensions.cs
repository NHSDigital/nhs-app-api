using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage
{
    internal static class CloudQueueMessageExtensions
    {
        internal static JToken AsJson(this CloudQueueMessage message)
        {
            return JsonConvert.DeserializeObject<JToken>(
                message.AsString,
                new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        }
    }
}