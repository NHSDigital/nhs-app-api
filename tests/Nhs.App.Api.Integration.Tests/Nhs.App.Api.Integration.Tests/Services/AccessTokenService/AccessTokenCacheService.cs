using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Nhs.App.Api.Integration.Tests.Services.AccessTokenService
{
    public class AccessTokenCacheService : IAccessTokenCacheService
    {
        private const int HttpTimeoutInSeconds = 30;
        private const int JwtLifespanInMinutes = 5;
        private const string MemoryCacheTokenAccessor = "TOKEN";

        private readonly HttpClient _httpClient;
        private readonly TestConfiguration _testConfiguration;
        private readonly IMemoryCache _memoryCache;

        public AccessTokenCacheService(
            TestConfiguration testConfiguration)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            }, true);

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _testConfiguration = testConfiguration;

            _httpClient.BaseAddress = new Uri(testConfiguration.TokenEndpoint);
            _httpClient.Timeout = new TimeSpan(0, 0, HttpTimeoutInSeconds);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task<string> FetchToken()
        {
            if (!_memoryCache.TryGetValue(MemoryCacheTokenAccessor, out string token))
            {
                var tokenModel = await GenerateAccessToken();

                // keep the value within cache for given amount of time, then delete it
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(
                        TimeSpan.FromSeconds(tokenModel.ExpiresAt));

                _memoryCache.Set(MemoryCacheTokenAccessor, tokenModel.Token, options);

                token = tokenModel.Token;
            }
            return token;
        }

        private async Task<JwtResponse> GenerateAccessToken()
        {
            var claim = _testConfiguration.IssuerKey;
            var claims = new List<Claim>
            {
                new("iss", claim),
                new("sub", claim),
                new("jti", Guid.NewGuid().ToString())
            };

            var token = GenerateNewTokenAsync(claims);

            var request = new HttpRequestMessage(HttpMethod.Post, _testConfiguration.TokenEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var parameters = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_assertion_type",
                    "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new("client_assertion", token.Token)
            };

            HttpContent content = new FormUrlEncodedContent(parameters);

            request.Content = content;

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None);
            var stream = await response.Content.ReadAsStreamAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException($"Could not acquire an access token - response code: {response.StatusCode}");

            var accessTokenResponse = stream.ReadAndDeserializeFromJson<AccessTokenDto>();
            return new JwtResponse
            {
                Token = accessTokenResponse.AccessToken,
                ExpiresAt = accessTokenResponse.ExpiresIn
            };
        }

        private JwtResponse GenerateNewTokenAsync(IEnumerable<Claim> claims)
        {
            using var rsa = RSA.Create();
            var privateKey = File.ReadAllText(_testConfiguration.PrivateKeyFilePath);
            rsa.ImportFromPem(privateKey);

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha512)
            {
                CryptoProviderFactory = new CryptoProviderFactory {CacheSignatureProviders = false}
            };

            var jwt = new JwtSecurityToken(_testConfiguration.IssuerKey,
                _testConfiguration.TokenEndpoint,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(JwtLifespanInMinutes),
                signingCredentials
            );
            jwt.Header.Add("kid", _testConfiguration.KidValue);

            var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            return new JwtResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                ExpiresAt = unixTimeSeconds,
            };
        }
    }
}
