using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class Im1TokenSerialiserService
    {
        private readonly JsonSerializerSettings _settings;

        public Im1TokenSerialiserService()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        internal string Serialise<T>(T token)
        {
            return JsonConvert.SerializeObject(token, _settings);
        }

        internal T Deserialise<T>(string tokenJson)
        {
            return JsonConvert.DeserializeObject<T>(tokenJson, _settings);
        }
    }
}