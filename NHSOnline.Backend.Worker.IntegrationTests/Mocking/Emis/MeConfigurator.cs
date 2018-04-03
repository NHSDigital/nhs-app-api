using System.Collections.Generic;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Shared;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis
{
    public static class MeConfigurator
    {
        private const string PathMe = "/emis/me";
        private const string PathApplications = PathMe + "/applications";

        public static Mapping CreateApplicationsMapping(int statusCode, Dictionary<string, string> headers, LinkApplicationRequest requestBody, LinkApplicationResponse linkApplicationResponse)
        {
            return new Mapping(
                new Request().ConfigureApplicationsRequest(headers, requestBody),
                new Response().ConfigureApplicationsResponse(statusCode, linkApplicationResponse)
            );
        }

        public static Request ConfigureApplicationsRequest(this Request request, Dictionary<string, string> headers, LinkApplicationRequest requestBody)
        {
            foreach (var header in headers)
            {
                request.ConfigureHeader(header.Key, header.Value);
            }

            return request
                .ConfigurePath(PathApplications)
                .ConfigureMethod(Methods.Post)
                .ConfigureApplicationsRequest(requestBody);
        }

        public static Request ConfigureApplicationsRequest(this Request request, LinkApplicationRequest requestBody)
        {
            request.ConfigureBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public static Response ConfigureApplicationsResponse(this Response response, int statusCode, LinkApplicationResponse responseBody)
        {
            return response
                .ConfigureStatusCode(statusCode)
                .ConfigureBody(JsonConvert.SerializeObject(responseBody));
        }
    }
}
