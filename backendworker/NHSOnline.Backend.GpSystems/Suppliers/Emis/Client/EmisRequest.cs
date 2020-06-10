using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Client
{
    internal sealed class EmisRequest: IEmisRequestBuilderType, IEmisRequestBuilderMethod, IEmisRequestBuilderContent, IEmisRequestBuilder, IDisposable
    {
        private const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        private const string HeaderSessionId = "X-API-SessionId";

        internal HttpRequestMessage RequestMessage { get; } = new HttpRequestMessage();
        internal List<HttpStatusCode> SuccessStatusCodes { get; } = new List<HttpStatusCode> { HttpStatusCode.OK };
        internal RequestsForSuccessOutcome Type { get; private set; }
        
        public IEmisRequestBuilderMethod RequestType(RequestsForSuccessOutcome requestType)
        {
            Type = requestType;
            return this;
        }

        public IEmisRequestBuilderContent Post(string uri) => Method(HttpMethod.Post, uri);

        public IEmisRequestBuilderContent Put(string uri) => Method(HttpMethod.Put, uri);

        public IEmisRequestBuilder Get(string uriFormat, params object[] args) => Method(HttpMethod.Get, uriFormat, args);

        public IEmisRequestBuilderContent Delete(string uri) => Method(HttpMethod.Delete, uri);
        public IEmisRequestBuilderContent Delete(string uriFormat, params object[] args) => Method(HttpMethod.Delete, uriFormat, args);

        private EmisRequest Method(HttpMethod method, string uriFormat, object[] args)
            => Method(method, string.Format(CultureInfo.InvariantCulture, uriFormat, args));

        private EmisRequest Method(HttpMethod method, string uri)
        {
            RequestMessage.Method = method;
            RequestMessage.RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute);
            return this;
        }

        public IEmisRequestBuilder Request<TRequest>(TRequest request)
        {
            var body = JsonConvert.SerializeObject(request);
            RequestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
            return this;
        }

        public IEmisRequestBuilder EmptyBody()
        {
            return this;
        }

        public IEmisRequestBuilder SessionId(string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                RequestMessage.Headers.Add(HeaderSessionId, new[] { sessionId });
            }

            return this;
        }

        public IEmisRequestBuilder EndUserSessionId(string endUserSessionId)
        {
            if (!string.IsNullOrEmpty(endUserSessionId))
            {
                RequestMessage.Headers.Add(HeaderEndUserSessionId, new[] { endUserSessionId });
            }

            return this;
        }

        public IEmisRequestBuilder Timeout(int timeout)
        {
            RequestMessage.Properties.Add(HttpRequestConstants.CustomTimeout, timeout);
            return this;
        }

        public IEmisRequestBuilder AdditionalSuccessHttpStatusCode(HttpStatusCode successCode)
        {
            SuccessStatusCodes.Add(successCode);
            return this;
        }

        public void Dispose()
        {
            RequestMessage.Dispose();
        }
    }
}