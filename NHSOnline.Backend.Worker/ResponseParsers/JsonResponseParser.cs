using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.ResponseParsers
{
    public class JsonResponseParser: BaseResponseParser, IJsonResponseParser
    {
        protected override T Deserialize<T>(string body)
        {
            return body.DeserializeJson<T>();
        }
    }
}