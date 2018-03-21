using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NHSOnline.Backend.Worker
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient(HttpClientName name);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly IDictionary<HttpClientName, HttpClient> _clients;

        public HttpClientFactory(IEnumerable<NamedHttpClient> clients)
        {
            _clients = clients.ToDictionary(n => n.Name, n => n.Client);
        }

        public HttpClient GetClient(HttpClientName name)
        {
            if (!_clients.TryGetValue(name, out var client))
                throw new ArgumentOutOfRangeException(nameof(name),
                    string.Format(ExceptionMessages.HttpClientFactoryUnknownClientName, name));

            return client;
        }
    }

    public enum HttpClientName
    {
        EmisApiClient
    }

    public class NamedHttpClient
    {
        public NamedHttpClient(HttpClientName name, HttpClient client)
        {
            Name = name;
            Client = client;
        }

        public HttpClientName Name { get; }
        public HttpClient Client { get; }
    }
}