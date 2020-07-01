using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NHSOnline.App.Api.Client
{
    internal sealed class JsonRequestContentSerialiser
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonRequestContentSerialiser(JsonSerializerOptions jsonSerializerOptions)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        internal Task SetContent<TRequestModel>(HttpRequestMessage requestMessage, TRequestModel model)
        {
            var json = JsonSerializer.Serialize(model, _jsonSerializerOptions);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return Task.CompletedTask;
        }
    }
}
