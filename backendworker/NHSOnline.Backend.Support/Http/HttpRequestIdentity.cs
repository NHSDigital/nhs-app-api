using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NHSOnline.Backend.Support.Http
{
    public class HttpRequestIdentity
    {
        private readonly Dictionary<string, string> _identifiers;

        public SourceApi SourceApi { get; set; }

        public HttpRequestIdentity(string provider, HttpRequestMessage request, SourceApi sourceApi)
        {
            SourceApi = sourceApi;
            _identifiers = new Dictionary<string, string>
            {
                { "Provider", provider },
                { "UpStreamMethod", request.Method?.ToString() },
                { "UpStreamUrl", request.RequestUri?.ToString() }
            };
        }
        
        public HttpRequestIdentity(string provider, string olcSessionId, HttpRequestMessage request, SourceApi sourceApi)
            : this(provider, request, sourceApi)
        {
            _identifiers["OlcSessionId"] = olcSessionId;
        }

        public HttpRequestIdentity SetUpStreamIdentifier(string upStreamIdentifier)
        {
            _identifiers["UpStreamIdentifier"]
                = string.IsNullOrWhiteSpace(upStreamIdentifier) ? null : upStreamIdentifier;
            return this;
        }

        public HttpRequestIdentity SetCorrelationIdentifier(string correlationIdentifier)
        {
            _identifiers["CorrelationIdentifier"] 
                = string.IsNullOrWhiteSpace(correlationIdentifier) ? null : correlationIdentifier;
            return this;
        }

        public override string ToString()
        {
            return string.Join(' ', _identifiers.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}