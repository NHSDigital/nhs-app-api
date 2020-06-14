using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Settings;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    internal sealed class EmisClientTestsContext : IDisposable
    {
        internal const string DefaultEmisVersion = "2.1.0.0";
        internal static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();

        internal static readonly Uri BaseUri = new Uri("http://emis_base_url/");

        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CertificatePassphrase";

        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        
        internal EmisClientTestsContext(int emisExtendedHttpTimeoutSeconds = EmisExtendedHttpTimeoutSeconds)
        {
            var services = new ServiceCollection();
            services.RegisterEmisCidServices();

            new Support.ResponseParsers.ServiceConfigurationModule().ConfigureServices(services, Configuration.Object);
            new Support.Temporal.ServiceConfigurationModule().ConfigureServices(services, Configuration.Object);

            var emisConfig = new EmisConfigurationSettings(
                BaseUri,
                DefaultEmisApplicationId,
                DefaultEmisVersion,
                CertificatePath,
                CertificatePassphrase,
                emisExtendedHttpTimeoutSeconds,
                DefaultHttpTimeoutSeconds,
                CoursesMaxCoursesLimit,
                PrescriptionsMaxCoursesSoftLimit);

            services.AddSingleton(emisConfig);

            services.AddSingleton(Configuration.Object);

            services.AddSingleton(MockHttpHandler);
            services.ReplacePrimaryHttpMessageHandler<EmisHttpClient, MockHttpMessageHandler>();

            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            var timeoutConfiguration = new Mock<IHttpTimeoutConfigurationSettings>();
            timeoutConfiguration.Setup(x => x.DefaultHttpTimeoutSeconds).Returns(10);
            services.AddSingleton(timeoutConfiguration.Object);

            ServiceProvider = services.BuildServiceProvider();
            SystemUnderTest = ServiceProvider.GetRequiredService<IEmisClient>();
        }

        internal Mock<IConfiguration> Configuration { get; } = new Mock<IConfiguration>();

        internal MockHttpMessageHandler MockHttpHandler { get; } = new MockHttpMessageHandler();

        internal IEmisClient SystemUnderTest { get; }

        internal IServiceProvider ServiceProvider { get; }

        public void Dispose() => MockHttpHandler.Dispose();
    }
}