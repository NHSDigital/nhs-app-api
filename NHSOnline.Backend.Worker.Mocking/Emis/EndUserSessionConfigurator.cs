using System;
using System.Net;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.Emis
{
    public class EndUserSessionConfigurator
    {
        private const string PathEndUserSession = "/emis/sessions/endusersession";

        private readonly Request _request;
        private readonly Response _response;

        private EndUserSessionConfigurator(Request request)
        {
            _request = request;
            _response = new Response();
        }

        public static EndUserSessionConfigurator ForRequest()
        {
            return new EndUserSessionConfigurator(CreateEndUserSessionRequest());
        }

        private static Request CreateEndUserSessionRequest()
        {
            return new Request()
                .ConfigurePath(PathEndUserSession)
                .ConfigureMethod(Methods.Post)
                .ConfigureApplicationHeader()
                .ConfigureVersionHeader();
        }

        public Mapping RespondWithSuccess(string endUserSessionId)
        {
            var responseBody = new EndUserSessionResponse(endUserSessionId);

            _response
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithServiceUnavailable()
        {
            _response
                .ConfigureStatusCode(HttpStatusCode.ServiceUnavailable);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithBadGatewayError()
        {
            _response
                .ConfigureStatusCode(HttpStatusCode.BadGateway);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithDelayedSuccess(string endUserSessionId, int delaySeconds)
        {
            var responseBody = new EndUserSessionResponse(endUserSessionId);

            _response
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureDelay(delaySeconds)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, _response);
        }

        public EndUserSessionConfigurator AddDelay(TimeSpan delay)
        {
            _response.FixedDelayMilliseconds = (int) delay.TotalMilliseconds;
            return this;
        }
    }
}
