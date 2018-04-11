using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Mocking
{
    public static class RequestConfigurator
    {
        public static Request ConfigurePath(this Request request, string path)
        {
            request.UrlPath = path;
            return request;
        }

        public static Request ConfigureMethod(this Request request, string method)
        {
            request.Method = method;
            return request;
        }

        public static Request ConfigureQueryParameter(this Request request, string name, string condition, string value)
        {
            return ConfigureQueryParameter(request, name, new Dictionary<string, string>{{ condition, value }});
        }

        public static Request ConfigureQueryParameter(this Request request, string name, Dictionary<string, string> options)
        {
            request.QueryParameters.Add(name, options);
            return request;
        }

        public static Request ConfigureHeader(this Request request, string header, Dictionary<string, string> options)
        {
            request.Headers.Add(header, options);

            return request;
        }

        public static Request ConfigureHeader(this Request request, string header, string value)
        {
            return ConfigureHeader(request, header, new Dictionary<string, string>{{ "equalTo", value } } );
        }

        public static Request ConfigureBody(this Request request, string condition, string body)
        {
            request.BodyPatterns.Add(new Dictionary<string, string> { { condition, body } });

            return request;
        }

        public static Request ConfigureBody(this Request request, string jsonBody)
        {
            return ConfigureBody(request, "equalToJson", jsonBody);
        }

    }
}
