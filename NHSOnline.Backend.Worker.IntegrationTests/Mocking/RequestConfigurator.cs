using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking
{
    public static class RequestConfigurator
    {
        public static Request ConfigurePath(this Request request, string path)
        {
            request.Path = path;
            return request;
        }

        public static Request ConfigureMethod(this Request request, string method)
        {
            request.Methods = new List<string> { method };
            return request;
        }

        public static Request ConfigureQueryParameter(this Request request, string name, string value)
        {
            return request.ConfigureQueryParameter(name, new List<string> { value });
        }

        public static Request ConfigureQueryParameter(this Request request, string name, IList<string> values)
        {
            request.Params = request.Params ?? new List<Param>();
            request.Params.Add(new Param { Name = name, Values = values });
            return request;
        }

        public static Request ConfigureHeader(this Request request, string name, string value)
        {
            request.Headers = request.Headers ?? new List<Header>();
            request.Headers.Add(new Header
            {
                Name = name,
                Matchers = new List<Matcher>
                {
                    new WildcardMatcher
                    {
                        Pattern = value
                    }
                }
            });

            return request;
        }

        public static Request ConfigureBody(this Request request, IDictionary<string, string> matchProperties)
        {
            var matchers = matchProperties.Keys.Select(x => $"$..[?(@.{x}=='{matchProperties[x]}')]").ToList();
            request.Body = new Body
            {
                Matcher = new JsonPathMatcher { Patterns = matchers }
            };

            return request;
        }
    }
}
