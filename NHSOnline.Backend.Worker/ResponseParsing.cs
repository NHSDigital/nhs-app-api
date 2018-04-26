using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker
{
    public static class ResponseParsing
    {
        public static T Deserialize<T>(this string json)
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
        
        public static T ParseBadRequest<T>(this string stringResponse, HttpResponseMessage message)
        {
            return message.StatusCode != HttpStatusCode.BadRequest
                ? default(T)
                : Deserialize<T>(stringResponse);
        }
        
        public static T ParseBody<T>(this string stringResponse, HttpResponseMessage message)
        {
            return message.IsSuccessStatusCode ? stringResponse.Deserialize<T>() : default(T);
        }


        public static T ParseError<T>(
            this string stringResponse, 
            HttpResponseMessage message, 
            params HttpStatusCode[] allowedErrorStatuses)
        {
            
            return message.IsSuccessStatusCode || allowedErrorStatuses.Any(x => x == message.StatusCode)
                ? default(T)
                : Deserialize<T>(stringResponse);
        }
    }
}