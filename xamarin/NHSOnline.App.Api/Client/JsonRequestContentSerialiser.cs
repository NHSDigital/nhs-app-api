using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NHSOnline.App.Api.Client
{
    internal sealed class JsonRequestContentSerialiser
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonRequestContentSerialiser(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        internal Task SetContent<TRequestModel>(HttpRequestMessage requestMessage, TRequestModel model)
        {
            var json = JsonConvert.SerializeObject(model, _jsonSerializerSettings);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return Task.CompletedTask;
        }
    }
}
