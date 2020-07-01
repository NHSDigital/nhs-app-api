using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.Sections;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
{
    internal class VisionPatientRecordSectionServiceTestContext
    {
        private static readonly Uri MockUri = new Uri("http://mockVision/", UriKind.Absolute);

        public VisionUserSession VisionUserSession { get; }
        internal VisionPatientRecordService SystemUnderTest { get;}
        public Mock<IHtmlSanitizer> MockSanitizer { get; }
        public Mock<IVisionClient> VisionClient { get;}


        public VisionPatientRecordSectionServiceTestContext()
        {
            VisionUserSession = new VisionUserSession();
            var logger = new Mock<ILogger<VisionPatientRecordService>>();
            VisionClient = new Mock<IVisionClient>();
            MockSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);

            var recordSectionResolver = CreateRecordSectionResolver();

            var allergyMapper = new VisionAllergyMapper(new Mock<ILogger<VisionAllergyMapper>>().Object);
            var medicationMapper = new VisionMedicationMapper(new Mock<ILogger<VisionMedicationMapper>>().Object);
            var immunisationsMapper =
                new VisionImmunisationsMapper(new Mock<ILogger<VisionImmunisationsMapper>>().Object);
            var problemsMapper = new VisionProblemsMapper(new Mock<ILogger<VisionProblemsMapper>>().Object);

            var testResultsMapper = new VisionTestResultsMapper(new Mock<ILogger<VisionTestResultsMapper>>().Object,
                MockSanitizer.Object);
            var diagnosisMapper = new VisionDiagnosisMapper(new Mock<ILogger<VisionDiagnosisMapper>>().Object,
                MockSanitizer.Object);
            var examinationsMapper = new VisionExaminationsMapper(new Mock<ILogger<VisionExaminationsMapper>>().Object,
                MockSanitizer.Object);
            var proceduresMapper = new VisionProceduresMapper(new Mock<ILogger<VisionProceduresMapper>>().Object,
                MockSanitizer.Object);
            
            var allergySection = new AllergiesSection(allergyMapper);
            var medicationSection = new MedicationsSection(medicationMapper);
            var immunisationsSection = new ImmunisationsSection(immunisationsMapper);
            var problemsSection = new ProblemsSection(problemsMapper);
            var testResultsSection = new TestResultSection(testResultsMapper);
            var diagnosisSection = new DiagnosisSection(diagnosisMapper);
            var examinationsSection = new ExaminationsSection(examinationsMapper);
            var proceduresSection = new ProceduresSection(proceduresMapper);

            SystemUnderTest = new VisionPatientRecordService(
                logger.Object,
                new VisionMyRecordMapper(),
                recordSectionResolver,
                allergySection,
                medicationSection,
                immunisationsSection,
                problemsSection,
                testResultsSection,
                diagnosisSection,
                examinationsSection,
                proceduresSection
            );
        }

        private PatientRecordSectionResolver CreateRecordSectionResolver()
        {
            var visionConfig = CreateConfig();

            return new PatientRecordSectionResolver(
                new Mock<ILogger<PatientRecordSectionResolver>>().Object,
                VisionClient.Object,
                visionConfig,
                new VisionMyRecordSectionMapper());
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