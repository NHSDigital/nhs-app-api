using System.Collections.Generic;

namespace NHSOnline.App.Api.Client.Cookies
{
    public class CookieJar
    {
        private readonly List<ApiCookie> _cookies;
        public IReadOnlyList<ApiCookie> Cookies => _cookies;
        public CookieJar()
        {
            _cookies = new List<ApiCookie>();
        }

        public void Add(ApiCookie apiCookie)
        {
            _cookies.Add(apiCookie);
        }
    }
}