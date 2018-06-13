using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker
{
    public static class SerializationExtensions
    {
        public static string SerializeJson(this object toJson)
        {
            return null;
        }

        public static T DeserializeJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                // TODO: Check logging strategy
                Console.WriteLine($"JSON was null or empty when attempting to deserialize: {typeof(T).FullName}");
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when attempting to deserialize: {typeof(T).FullName}");
                Console.WriteLine("JSON");
                Console.WriteLine("--------------------");
                Console.WriteLine(json);
                Console.WriteLine();
                Console.WriteLine("EXCEPTION");
                Console.WriteLine("--------------------");
                Console.WriteLine(e);
                return default(T);
            }
        }
    }
}