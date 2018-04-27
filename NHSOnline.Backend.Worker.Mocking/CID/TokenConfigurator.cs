using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using NHSOnline.Backend.Worker.Mocking.Shared;

namespace NHSOnline.Backend.Worker.Mocking.CID
{
    public class TokenConfigurator
    {
        private const string PathToken = "/token";

        private readonly Request _request;
        private readonly Response _response;

        private TokenConfigurator(Request request)
        {
            _request = request;
            _response = new Response();
        }

        public static TokenConfigurator ForRequest(string authCode, string codeVerifier)
        {
            return new TokenConfigurator(CreateTokensRequest(authCode, codeVerifier));
        }

        private static Request CreateTokensRequest(
            string authCode, string codeVerifier)
        {
            var request = new TokenRequest(authCode, codeVerifier);

            var bodyProperties = ConvertDictionaryToString(request);

            return new Request()
                .ConfigurePath(PathToken)
                .ConfigureMethod(Methods.Post)
                .ConfigureTokensAuthorizationHeader("Basic") // We check the Header value contains Basic
                .ConfigureHeader("Content-Type", "application/x-www-form-urlencoded")
                .ConfigureBody("equalTo", bodyProperties );
        }

        public Mapping RespondWithSuccess(string accessToken)
        {
            var responseBody = new TokenResponse(accessToken);

            _response
                .ConfigureStatusCode(HttpStatusCode.OK)
                .ConfigureBodyObject(responseBody);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithBadRequest()
        {
            _response
                .ConfigureStatusCode(HttpStatusCode.BadRequest);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithServiceUnavailable()
        {
            const string responseBody = "Service Unavailable";

            _response
                .ConfigureStatusCode(HttpStatusCode.ServiceUnavailable)
                .ConfigureBody(responseBody);

            return new Mapping(_request, _response);
        }

        public Mapping RespondWithServerError()
        {
            const string responseBody = "Server Error";

            _response
                .ConfigureStatusCode(HttpStatusCode.InternalServerError)
                .ConfigureBody(responseBody);

            return new Mapping(_request, _response);
        }

        private static string ConvertDictionaryToString(TokenRequest request)
        {
            var dict = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", request.Code },
                { "redirect_uri", WebUtility.UrlEncode(request.Redirect_Uri) },
                { "code_verifier", request.Code_Verifier },
                { "client_id", request.Client_Id },
                { "code_challenge_method", "S256" }
            };

            // Converts dictionary to correct string format
            return string.Join("&", dict.Select(kv => kv.Key + "=" + kv.Value).ToArray());
        }
    }
}