using System;
using System.Text;
using Microsoft.Net.Http.Headers;

namespace NHSOnline.App.Api.Client.Cookies
{
    /// <summary>
    /// The various available cookie classes don't cover all of the attributes we need to pass through from API responses
    /// </summary>
    public class ApiCookie
    {
        public Uri Uri { get; }
        public string Value { get; }

        public ApiCookie(Uri uri, string cookieHeader)
        {
            Uri = uri;
            Value = cookieHeader;
        }

        public ApiCookie(Uri uri,
            string name,
            string value,
            string domain,
            string path,
            bool httpOnly,
            bool secure,
            SameSiteMode sameSite)
        {
            Uri = uri;

            Value = new CookieBuilder()
                .AddAttribute(name, value)
                .AddAttribute("Path", path)
                .AddAttribute("Domain", domain)
                .AddAttribute("SameSite", sameSite.ToString())
                .AddAttribute("HttpOnly", httpOnly)
                .AddAttribute("Secure", secure)
                .Build();
        }

        private class CookieBuilder
        {
            private readonly StringBuilder _cookie = new StringBuilder();

            internal CookieBuilder AddAttribute(string attributeKey, string attributeValue)
            {
                if (_cookie.Length != 0)
                {
                    _cookie.Append("; ");
                }

                _cookie.Append(attributeKey);
                _cookie.Append('=');
                _cookie.Append(attributeValue);

                return this;
            }

            internal CookieBuilder AddAttribute(string attributeKey, bool flagValue)
            {
                if (flagValue)
                {
                    if (_cookie.Length != 0)
                    {
                        _cookie.Append("; ");
                    }

                    _cookie.Append(attributeKey);
                }

                return this;
            }

            internal string Build() => _cookie.ToString();
        }
    }
}