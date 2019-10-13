using System;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.UserInfo
{
    public static class UserInfoHttpMockingExtensions
    {
        public static MockedRequest WhenUserInfo(this MockHttpMessageHandler handler, HttpMethod method,
            string relativePath)
        {
            var url = new Uri(UserInfoClientTests.BaseUri, relativePath);
            return handler.When(method, url.ToString());
        }
    }
}