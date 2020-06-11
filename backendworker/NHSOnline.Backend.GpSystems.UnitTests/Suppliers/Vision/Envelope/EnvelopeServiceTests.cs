using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope.ServiceEvaluators;
using NHSOnline.Backend.Support.Certificate;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope
{
    [TestClass]
    public sealed class EnvelopeServiceTests
    {
        [TestMethod]
        public void BuildEnvelope_GetConfiguration_CorrectFormat()
        {
            using var context = new EnvelopeServiceTestContext();

            const string expectedServiceDefinition = "VOS.GetConfiguration";
            const string expectedVersionDefinition = "2.3.0";

            const string expectedRosuAccountId = "104969";
            const string expectedApiKey = "xahry1234";
            const string expectedOdsCode = "X00100";
            const string expectedProviderId = "nhson001";
            const string requestUsername = "vps-nhson001";

            var visionRequest = new VisionRequest<object>(expectedServiceDefinition, expectedVersionDefinition,
                expectedRosuAccountId, expectedApiKey, expectedOdsCode, expectedProviderId, null);

            var result = context.SystemUnderTest.BuildEnvelope(visionRequest, requestUsername);

            result.Should().BeAssignableTo<string>()
                .Subject.Should().Contain("<vision:serviceContent />");

            var getConfiguration = new GetConfigurationEvaluator(result);

            getConfiguration.Validate(
                expectedServiceDefinition,
                expectedVersionDefinition,
                expectedRosuAccountId,
                expectedApiKey,
                expectedOdsCode,
                expectedProviderId);
        }

        // Test to ensure that the request body can be built using a generic ServiceContent
        [TestMethod]
        public void BuildEnvelope_GenericContentBody_CorrectFormat()
        {
            using var context = new EnvelopeServiceTestContext();

            const string expectedServiceDefinition = "VOS.GenericService";
            const string expectedVersionDefinition = "9.9.9";

            const string expectedRosuAccountId = "104969";
            const string expectedApiKey = "xahry1234";
            const string expectedOdsCode = "X00100";
            const string expectedProviderId = "nhson001";
            const string requestUsername = "vps-nhson001";

            const string expectedServiceContent = "<vision:serviceContent>" +
                                                  "<vision:vos>" +
                                                  "<vision:hello>myTest</vision:hello>" +
                                                  "</vision:vos>" +
                                                  "</vision:serviceContent>";

            var visionRequest = new VisionRequest<TestClass>(expectedServiceDefinition, expectedVersionDefinition,
                expectedRosuAccountId, expectedApiKey, expectedOdsCode, expectedProviderId,
                new TestClass { TestProp = "myTest" });

            var result = context.SystemUnderTest.BuildEnvelope(visionRequest, requestUsername);

            result.Should().Contain(expectedServiceContent);
        }

        private sealed class EnvelopeServiceTestContext : IDisposable
        {
            private const string CertificatePath = "Suppliers/Vision/Resources/mycert.pfx";
            private const string Password = "password1";

            private readonly X509Certificate2 _certificate;
            private readonly ServiceProvider _serviceProvider;

            internal EnvelopeServiceTestContext()
            {
                _certificate = new X509Certificate2(CertificatePath, Password);

                var mockCertificateService = new Mock<ICertificateService>();
                mockCertificateService
                    .Setup(x => x.GetCertificate(CertificatePath, Password))
                    .Returns(_certificate);

                var visionConfiguration = new VisionConfigurationSettings("", null, CertificatePath, Password, "", "", "", "", "", 0, null, null, "");

                var services = new ServiceCollection()
                    .RegisterVisionEnvelopeServices()
                    .AddMockLoggers()
                    .AddSingleton(mockCertificateService.Object)
                    .AddSingleton(visionConfiguration);

                _serviceProvider = services.BuildServiceProvider();
                SystemUnderTest = _serviceProvider.GetRequiredService<IEnvelopeService>();
            }

            public IEnvelopeService SystemUnderTest { get; }

            public void Dispose()
            {
                _certificate.Dispose();
                _serviceProvider.Dispose();
            }
        }

        public class TestClass
        {
            [XmlElement(ElementName = "hello", Namespace = "urn:vision")]
            public string TestProp { get; set; }
        }
    }
}
