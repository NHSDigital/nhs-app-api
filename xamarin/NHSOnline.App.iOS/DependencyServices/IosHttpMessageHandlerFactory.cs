using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosHttpMessageHandlerFactory))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosHttpMessageHandlerFactory : IPrimaryHttpMessageHandlerFactory
    {
        public HttpMessageHandler CreatePrimaryHttpMessageHandler()
        {
            return GetSessionHandler();
        }

        private static NSUrlSessionHandler GetSessionHandler()
        {
           return UIDevice.CurrentDevice.CheckSystemVersion(12, 0) ?
                new NsUrlSessionHandlerSameSiteCookieFix() :
                new NSUrlSessionHandler();
        }

        /// <summary>
        /// The Xamarin NSUrlSessionHandler class creates Set-Cookie headers in the HttpResponseMessage class based on
        /// NSHttpCookies that NSUrlSession extracts from the server responses. Unfortunately it fails to copy the
        /// SameSite attribute into the Set-Cookie header, as can be seen in the source on GitHub:
        /// https://github.com/xamarin/xamarin-macios/blob/main/src/Foundation/NSUrlSessionHandler.cs#L71
        ///
        /// This class works around this limitation and fixup the Set-Cookie headers to include the SameSite attribute.
        /// </summary>
        private class NsUrlSessionHandlerSameSiteCookieFix : NSUrlSessionHandler
        {
            private readonly NSHttpCookieStorage _cookieStorage;
            private static ILogger Logger => NhsAppLogging.CreateLogger<NsUrlSessionHandlerSameSiteCookieFix>();

            public NsUrlSessionHandlerSameSiteCookieFix() :this (NSHttpCookieStorage.SharedStorage)
            { }

            private NsUrlSessionHandlerSameSiteCookieFix(NSHttpCookieStorage cookieStorage) : base(CreateConfig(cookieStorage))
            {
                _cookieStorage = cookieStorage;
            }

            private static NSUrlSessionConfiguration CreateConfig(NSHttpCookieStorage cookieStorage)
            {
                var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;

                // The value of these timeouts are copied from the base class so as not to change the behaviour
                config.TimeoutIntervalForRequest = TimeSpan.FromDays(1).Seconds;
                config.TimeoutIntervalForResource = TimeSpan.FromDays(1).Seconds;

                config.HttpCookieStorage = cookieStorage;

                return config;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken).ResumeOnThreadPool();

                FixupCookies(request, response);

                return response;
            }

            private void FixupCookies(HttpRequestMessage request, HttpResponseMessage response)
            {
                const string setCookieHeaderKey = "Set-Cookie";

                if (response.Headers.TryGetValues(setCookieHeaderKey, out var cookieHeaders))
                {
                    response.Headers.Remove(setCookieHeaderKey);

                    var cookiesForUrl = _cookieStorage.CookiesForUrl(request.RequestUri);
                    cookieHeaders = CopySameSiteAttribute(cookieHeaders, cookiesForUrl);

                    response.Headers.TryAddWithoutValidation(setCookieHeaderKey, cookieHeaders);
                }
            }

            private static IEnumerable<string> CopySameSiteAttribute(IEnumerable<string> cookieHeadersEnumerable, IEnumerable<NSHttpCookie> cookies)
            {
                var cookieHeaders = cookieHeadersEnumerable.ToList();

                foreach (var cookie in cookies)
                {
                    CopySameSiteAttribute(cookie, cookieHeaders);
                }

                return cookieHeaders;
            }

            private static void CopySameSiteAttribute(NSHttpCookie fromCookie, List<string> toCookieHeaders)
            {
                try
                {
                    if (fromCookie.SameSitePolicy != null)
                    {
                        ReplaceFirstStringThatMatches(
                            toCookieHeaders,
                            x => x.StartsWith(fromCookie.Name, StringComparison.Ordinal),
                            x => $"{x}; SameSite={fromCookie.SameSitePolicy}");
                    }
                }
                catch (MonoTouchException e)
                {
                    Logger.LogError(e, "Failed to copy SameSite attribute of cookie");
                }
            }

            private static void ReplaceFirstStringThatMatches(
                List<string> values,
                Predicate<string> isMatch,
                Func<string, string> replacer)
            {
                var matchingIndex = values.FindIndex(isMatch);
                values[matchingIndex] = replacer(values[matchingIndex]);
            }
        }
    }
}