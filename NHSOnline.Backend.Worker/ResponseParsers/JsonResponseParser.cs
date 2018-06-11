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
            if (string.IsNullOrEmpty(body))
            {
                // TODO: Check logging strategy
                Console.WriteLine($"JSON was null or empty when attempting to deserialize: {typeof(T).FullName}");
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when attempting to deserialize: {typeof(T).FullName}");
                Console.WriteLine("JSON");
                Console.WriteLine("--------------------");
                Console.WriteLine(body);
                Console.WriteLine();
                Console.WriteLine("EXCEPTION");
                Console.WriteLine("--------------------");
                Console.WriteLine(e);
                return default(T);
            }
        }
    }
}