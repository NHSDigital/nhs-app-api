using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope.ServiceEvaluators;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope
{
    [TestClass]
    public sealed class EnvelopeServiceTests : IDisposable
    {
        private IFixture _fixture;

        private EnvelopeService _systemUnderTest;
        private X509Certificate2 _certificate;
        private const string CertificatePath = "Suppliers/Vision/Resources/mycert.pfx";
        private const string Password = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _certificate = new X509Certificate2(CertificatePath, Password);
            _systemUnderTest = _fixture.Create<EnvelopeService>();
        }

        [TestMethod]
        public void BuildEnvelope_GetConfiguration_CorrectFormat()
        {
            const string expectedServiceDefinition = "VOS.GetConfiguration";
            const string expectedVersionDefinition = "2.3.0";

            const string expectedRosuAccountId = "104969";
            const string expectedApiKey = "xahry1234";
            const string expectedOdsCode = "X00100";
            const string expectedProviderId = "nhson001";
            const string requestUsername = "vps-nhson001";

            var visionRequest = new VisionRequest<Object>(expectedServiceDefinition, expectedVersionDefinition,
                expectedRosuAccountId, expectedApiKey, expectedOdsCode, expectedProviderId, null);

            var result = _systemUnderTest.BuildEnvelope(_certificate,
                visionRequest,
                requestUsername);

            result.Should().BeAssignableTo<String>();
            result.Should().Contain("<vision:serviceContent />");

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

            var result = _systemUnderTest.BuildEnvelope(_certificate,
                visionRequest,
                requestUsername);

            result.Should().Contain(expectedServiceContent);
        }

        public void Dispose()
        {
            _certificate.Dispose();
        }
    }
}
