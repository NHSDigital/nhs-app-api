using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class JsonResponseParser
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonResponseParser(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        internal async Task<T> Parse<T>(HttpResponseMessage responseMessage)
        {
            var responseStream = await responseMessage.Content.ReadAsStreamAsync().ResumeOnThreadPool();
            return await JsonSerializer.DeserializeAsync<T>(responseStream, _jsonSerializerOptions).ResumeOnThreadPool();
        }
    }
}