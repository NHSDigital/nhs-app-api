using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class JsonResponseParser
    {
        private readonly JsonSerializerSettings _jsonSerializerOptions;

        public JsonResponseParser(JsonSerializerSettings jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        internal async Task<T> Parse<T>(HttpResponseMessage responseMessage)
        {
            var responseBody = await responseMessage.Content.ReadAsStringAsync().ResumeOnThreadPool();
            return JsonConvert.DeserializeObject<T>(responseBody, _jsonSerializerOptions) ??
                   throw new InvalidOperationException($"Failed to parse response as {typeof(T).FullName}");
        }
    }
}