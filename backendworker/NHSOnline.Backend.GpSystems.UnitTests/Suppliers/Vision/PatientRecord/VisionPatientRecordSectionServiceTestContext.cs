using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Sanitization;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
{
    internal class VisionPatientRecordSectionServiceTestContext
    {
        private static readonly Uri MockUri = new Uri("http://mockVision/", UriKind.Absolute);

        public VisionUserSession VisionUserSession { get; } = new VisionUserSession();
        internal IVisionPatientRecordService SystemUnderTest { get;}
        internal Mock<IHtmlSanitizer> MockSanitizer { get; }
        internal Mock<IVisionClient> VisionClient { get;}
        
        public VisionPatientRecordSectionServiceTestContext()
        {
            var logger = new Mock<ILogger<VisionPatientRecordService>>();
            VisionClient = new Mock<IVisionClient>();
            MockSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);

            var services = new ServiceCollection()
                .RegisterVisionPatientRecordServices()
                .AddSingleton(logger.Object)
                .AddSingleton(VisionClient.Object)
                .AddSingleton(MockSanitizer.Object)
                .AddSingleton(CreateConfig())
                .AddMockLoggers();

            SystemUnderTest = services.BuildServiceProvider(true).GetRequiredService<VisionPatientRecordService>();
        }

        private VisionConfigurationSettings CreateConfig()
        {
            return new VisionConfigurationSettings("",
                MockUri,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                1,
                null,
                null);
        }

        public VisionPfsApiObjectResponse<VisionPatientDataResponse> CreateVisionResponse(string sanitizedHtml, VisionPatientRecordSectionServiceTestContext context)
        {
            context.MockSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(sanitizedHtml);

            var data = new VisionPatientDataResponse()
            {
                Record = sanitizedHtml
            };
            var visionResponse = new VisionPfsApiObjectResponse<VisionPatientDataResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<VisionPatientDataResponse>
                {
                    Body = new VisionResponseBody<VisionPatientDataResponse>
                    {
                        VisionResponse = new VisionResponse<VisionPatientDataResponse>
                            { ServiceContent = data }
                    }
                }
            };
            return visionResponse;
        }

        public string CreateHtml(string expectedSectionName)
        {
            return
                $"<!DOCTYPE html>\n<html><div class=\"inps_class_single_result_frame\"><h3>{expectedSectionName}</h3></div></html>";
        }

        public void MockVisionResponse(string expectedSectionName, VisionPatientRecordSectionServiceTestContext context)
        {
            var sanitizedHtml = CreateHtml(expectedSectionName);
            var mockVisionResponse = CreateVisionResponse(sanitizedHtml, context);
            context.VisionClient.Setup(x => x.GetPatientData(context.VisionUserSession,
                    It.Is<PatientDataRequest>(x => x.View == expectedSectionName)))
                .ReturnsAsync(mockVisionResponse);
        }
    }
}