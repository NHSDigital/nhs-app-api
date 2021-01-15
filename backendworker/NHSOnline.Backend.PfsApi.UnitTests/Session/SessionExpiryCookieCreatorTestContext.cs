using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Session
{
    internal class SessionExpiryCookieCreatorTestContext
    {
        private const int SessionExpiryMinutes = 10;
        internal const string JwtToken = "Jwt.To.Ken";

        private TestMocks Mocks { get; }
        private TestData Data { get; }
        private ServiceProvider ServiceProvider { get; set; }

        public SessionExpiryCookieCreatorTestContext()
        {
            Mocks = new TestMocks();
            Data = new TestData();
            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(Data.ConfigurationSettings)
                .AddSingleton(Data.AuthSigningConfig)
                .AddTransient<SessionExpiryCookieCreator>();

            Mocks.ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            Mocks.HttpContext.Setup(x => x.RequestServices).Returns(() => ServiceProvider);
        }

        internal SessionExpiryCookieCreator CreateSystemUnderTest() =>
            ServiceProvider.GetRequiredService<SessionExpiryCookieCreator>();

        internal void ArrangeSigning()
        {
            Mocks.Signing
                .Setup(s => s.GetRsaParameters(Data.AuthSigningConfig))
                .Returns(Data.RsaParameters);
        }

        internal void ArrangeSigningException()
        {
            Mocks.Signing
                .Setup(s => s.GetRsaParameters(Data.AuthSigningConfig))
                .Throws<IOException>();
        }

        internal void ArrangeJwtTokenGenerator(string token = JwtToken)
        {
            Mocks.JwtTokenGenerator
                .Setup(j => j.GenerateJwtSecurityToken(
                    Data.RsaParameters,
                    It.Is<Dictionary<string, object>>(d =>
                        d.ContainsKey("exp") &&
                        (DateTime) d["exp"] == Data.UtcNow.AddMinutes(Data.ConfigurationSettings.DefaultSessionExpiryMinutes) &&
                        d.Keys.Count == 1)))
                .Returns(token);
        }

        internal void ArrangeDateTimeUtcNow()
        {
            Mocks.CurrentDateTimeProvider
                .Setup(c => c.UtcNow)
                .Returns(Data.UtcNow);
        }

        private class TestData
        {
            internal ConfigurationSettings ConfigurationSettings { get; }
            internal RSAParameters RsaParameters { get; }
            internal DateTime UtcNow { get; }
            internal AuthSigningConfig AuthSigningConfig { get; }

            internal TestData()
            {
                var configuration = new ConfigurationStub(new Dictionary<string, string>
                {
                    { "AUTH_SIGNING_KEY", "AuthSigningKey" },
                    { "AUTH_SIGNING_PASSWORD", "AuthSigningPassword" }
                });

                UtcNow = DateTime.UtcNow;

                RsaParameters = new RSAParameters();

                ConfigurationSettings = new ConfigurationSettings
                {
                    DefaultSessionExpiryMinutes = SessionExpiryMinutes
                };

                AuthSigningConfig = new AuthSigningConfig(configuration, new Mock<ILogger<AuthSigningConfig>>().Object);
            }
        }

        internal class TestMocks
        {
            internal Mock<HttpContext> HttpContext { get; } = new Mock<HttpContext>();
            internal Mock<ISigning> Signing { get; } = new Mock<ISigning>();
            internal Mock<ICurrentDateTimeProvider> CurrentDateTimeProvider { get; } = new Mock<ICurrentDateTimeProvider>();
            internal Mock<IJwtTokenGenerator> JwtTokenGenerator { get; } = new Mock<IJwtTokenGenerator>();

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Signing.Object)
                    .AddSingleton(CurrentDateTimeProvider.Object)
                    .AddSingleton(JwtTokenGenerator.Object)
                    .AddSingleton(new Mock<IWebHostEnvironment>().Object)
                    .AddMockLoggers();
            }
        }
    }
}