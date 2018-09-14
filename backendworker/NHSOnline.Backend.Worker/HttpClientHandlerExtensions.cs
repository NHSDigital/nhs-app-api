using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker
{
    public static class HttpClientHandlerExtensions
    {
        private const string HttpsProxyVariableLower = "https_proxy";
        private const string HttpsProxyVariableUpper = "HTTPS_PROXY";
        
        private const string NoProxyVariableLower = "no_proxy";
        private const string NoProxyVariableUpper = "NO_PROXY";
        
        public static HttpClientHandler ConfigureForwardProxy(this HttpClientHandler httpClientHandler, IConfiguration configuration)
        {
            var httpsProxy = configuration.GetFirstOrDefault(HttpsProxyVariableLower, HttpsProxyVariableUpper);
            if (string.IsNullOrEmpty(httpsProxy))
            {
                return httpClientHandler;
            }

            var bypassList = GetBypassList(configuration);

            httpClientHandler.UseProxy = true;
            httpClientHandler.Proxy = bypassList == null ? 
                new WebProxy(httpsProxy, true) : 
                new WebProxy(httpsProxy, true, bypassList); 
            
            httpClientHandler.Proxy = new WebProxy(httpsProxy, true, bypassList);

            return httpClientHandler;
        }

        private static string[] GetBypassList(IConfiguration configuration)
        {
            var noProxy = configuration.GetFirstOrDefault(NoProxyVariableLower, NoProxyVariableUpper);

            if (string.IsNullOrEmpty(noProxy))
            {
                return null;
            }
            
            return noProxy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}