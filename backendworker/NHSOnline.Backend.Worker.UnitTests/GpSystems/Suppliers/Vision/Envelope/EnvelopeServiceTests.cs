using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Envelope.ServiceEvaluators;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Envelope
{
    [TestClass]
    public sealed class EnvelopeServiceTests : IDisposable
    {
        private IFixture _fixture;

        private EnvelopeService _systemUnderTest;
        private X509Certificate2 _certificate;
        private const string certificatePath = "GpSystems/Suppliers/Vision/Resources/mycert.pfx";
        private const string password = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _certificate = new X509Certificate2(certificatePath, password);
            _systemUnderTest = _fixture.Create<EnvelopeService>();
        }

        [TestMethod]
        public void BuildEnvelope_GetConfiguration_CorrectFormat()
        {
            const string ExpectedServiceDefinition = "VOS.GetConfiguration";
            const string ExpectedVersionDefinition = "2.3.0";

            const string ExpectedRosuAccountId = "104969";
            const string ExpectedApiKey = "xahry1234";
            const string ExpectedOdsCode = "X00100";
            const string ExpectedProviderId = "nhson001";
            const string RequestUsername = "vps-nhson001";

            var visionRequest = new VisionRequest<Object>(ExpectedServiceDefinition, ExpectedVersionDefinition,
                ExpectedRosuAccountId, ExpectedApiKey, ExpectedOdsCode, ExpectedProviderId, null);

            var result = _systemUnderTest.BuildEnvelope(_certificate,
                visionRequest,
                RequestUsername);

            result.Should().BeAssignableTo<String>();
            result.Should().Contain("<vision:serviceContent />");

            var getConfiguration = new GetConfigurationEvaluator(result);

            getConfiguration.Validate(
                ExpectedServiceDefinition,
                ExpectedVersionDefinition,
                ExpectedRosuAccountId,
                ExpectedApiKey,
                ExpectedOdsCode,
                ExpectedProviderId);
        }

        // Test to ensure that the request body can be built using a generic ServiceContent
        [TestMethod]
        public void BuildEnvelope_GenericContentBody_CorrectFormat()
        {
            const string ExpectedServiceDefinition = "VOS.GenericService";
            const string ExpectedVersionDefinition = "9.9.9";

            const string ExpectedRosuAccountId = "104969";
            const string ExpectedApiKey = "xahry1234";
            const string ExpectedOdsCode = "X00100";
            const string ExpectedProviderId = "nhson001";
            const string RequestUsername = "vps-nhson001";

            const string expectedServiceContent = "<vision:serviceContent>" +
                                                  "<vision:vos>" +
                                                  "<vision:hello>myTest</vision:hello>" +
                                                  "</vision:vos>" +
                                                  "</vision:serviceContent>";

            var visionRequest = new VisionRequest<TestClass>(ExpectedServiceDefinition, ExpectedVersionDefinition,
                ExpectedRosuAccountId, ExpectedApiKey, ExpectedOdsCode, ExpectedProviderId,
                new TestClass { TestProp = "myTest" });

            var result = _systemUnderTest.BuildEnvelope(_certificate,
                visionRequest,
                RequestUsername);

            result.Should().Contain(expectedServiceContent);
        }

        public void Dispose()
        {
            _certificate.Dispose();
        }
    }
}
