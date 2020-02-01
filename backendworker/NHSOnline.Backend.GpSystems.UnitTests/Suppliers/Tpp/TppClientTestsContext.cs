using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Settings;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    internal sealed class TppClientTestsContext: IDisposable
    {
        internal const string ApplicationName = "appName";
        internal const string ApplicationVersion = "13";
        internal const string ApplicationProviderId = "providerId";
        internal const string ApplicationDeviceType = "deviceType";
        internal const string ApiVersion = "12";
        internal const string CertificatePath = "CertificatePath";
        internal const string CertificatePassphrase = "CerticiatePassphrase";
        internal const string Environment = "environment";

        internal static readonly Guid Uuid = new Guid("8a1c6b80-7bcb-49fd-9c6f-4801e12207d6");
        internal static readonly Uri ApiUrl = new Uri("http://tppapitest:60015/Test/");
        internal static readonly int? PrescriptionsMaxCoursesSoftLimit = 100;
        internal static readonly int? CoursesMaxCoursesLimit = 100;

        internal Mock<ILogger<TppClientRequestExecutor>> MockLogger { get; } = new Mock<ILogger<TppClientRequestExecutor>>();

        internal MockHttpMessageHandler MockHttpHandler { get; } = new MockHttpMessageHandler();

        internal ServiceProvider ServiceProvider { get; private set; }

        internal void Initialise()
        {
            var services = new ServiceCollection();
            services.RegisterTppCidServices();

            var configuration = new Mock<IConfiguration>().Object;
            new Support.ResponseParsers.ServiceConfigurationModule().ConfigureServices(services, configuration);
            services.AddSingleton(configuration);

            var mockGuidCreator = new Mock<IGuidCreator>();
            mockGuidCreator.Setup(x => x.CreateGuid()).Returns(TppClientTestsContext.Uuid);
            services.AddSingleton(mockGuidCreator.Object);

            services.AddSingleton(MockLogger.Object);

            var tppConfig = new TppConfigurationSettings(TppClientTestsContext.ApiUrl, TppClientTestsContext.ApiVersion, TppClientTestsContext.ApplicationName, TppClientTestsContext.ApplicationVersion, TppClientTestsContext.ApplicationProviderId, TppClientTestsContext.ApplicationDeviceType, TppClientTestsContext.CertificatePassphrase, TppClientTestsContext.CertificatePath, TppClientTestsContext.PrescriptionsMaxCoursesSoftLimit, TppClientTestsContext.CoursesMaxCoursesLimit, TppClientTestsContext.Environment);
            services.AddSingleton(tppConfig);

            services.AddSingleton(MockHttpHandler);
            services.ReplacePrimaryHttpMessageHandler<TppHttpClient, MockHttpMessageHandler>();

            services.AddSingleton(typeof(HttpTimeoutHandler<>));
            services.AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            var timeoutConfiguration = new Mock<IHttpTimeoutConfigurationSettings>();
            timeoutConfiguration.Setup(x => x.DefaultHttpTimeoutSeconds).Returns(10);
            services.AddSingleton(timeoutConfiguration.Object);

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose() => MockHttpHandler.Dispose();
    }
}