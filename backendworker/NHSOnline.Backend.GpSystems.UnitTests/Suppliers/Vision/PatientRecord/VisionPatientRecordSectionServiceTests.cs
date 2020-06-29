using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionPatientRecordSectionServiceTests
    {
        [TestMethod]
        public async Task GetMyRecord_Successful()
        {
            var context = new VisionPatientRecordSectionServiceTestContext();
            context.MockVisionResponse("VPS_ALLERGIES", context);
            context.MockVisionResponse("VPS_MEDICATIONS", context);
            context.MockVisionResponse("PROBLEMS", context);
            context.MockVisionResponse("DIAGNOSIS", context);
            context.MockVisionResponse("MEDICATIONS", context);
            context.MockVisionResponse("PROCEDURES", context);
            context.MockVisionResponse("TEST RESULTS", context);
            context.MockVisionResponse("EXAM FINDINGS", context);

            // Act
            var result = await context.SystemUnderTest.GetMyRecord(new GpLinkedAccountModel(context.VisionUserSession));

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.Success>();
        }

        [DataTestMethod]
        [DataRow(VisionMapperType.TestResults, "TEST RESULTS")]
        [DataRow(VisionMapperType.Examinations, "EXAM FINDINGS")]
        [DataRow(VisionMapperType.Diagnosis, "DIAGNOSIS")]
        [DataRow(VisionMapperType.Procedures, "PROCEDURES")]
        public async Task GetSection_Successful(VisionMapperType section, string expectedSectionName)
        {
            var context = new VisionPatientRecordSectionServiceTestContext();
            var sanitizedHtml = context.CreateHtml(expectedSectionName);
            var mockVisionResponse = context.CreateVisionResponse(sanitizedHtml, context);

            context.VisionClient.Setup(x => x.GetPatientData(context.VisionUserSession,
                    It.Is<PatientDataRequest>(x => x.View == expectedSectionName)))
                .ReturnsAsync(mockVisionResponse);

            // Act
            var result = await context.SystemUnderTest.GetSection(context.VisionUserSession, section);

            // Assert
            var response = result.Should().BeAssignableTo<GetMyRecordSectionResult.Success>()
                .Subject.Response;
            response.SectionName.Should().Be(expectedSectionName);
            response.Markup.Should().BeEquivalentTo(sanitizedHtml);
        }

        [DataTestMethod]
        [DataRow(VisionMapperType.Medications, "VPS_MEDICATIONS")]
        [DataRow(VisionMapperType.Allergies, "VPS_ALLERGIES")]
        [DataRow(VisionMapperType.Immunisations, "IMMUNISATIONS")]
        [DataRow(VisionMapperType.Problems, "PROBLEMS")]
        public async Task GetSection_SectionNotAllowed_ReturnsBadRequest(VisionMapperType section,
            string expectedSectionName)
        {
            var context = new VisionPatientRecordSectionServiceTestContext();
            var sanitizedHtml = context.CreateHtml(expectedSectionName);
            var mockVisionResponse = context.CreateVisionResponse(sanitizedHtml, context);
            context.VisionClient.Setup(x => x.GetPatientData(context.VisionUserSession,
                    It.Is<PatientDataRequest>(x => x.View == expectedSectionName)))
                .ReturnsAsync(mockVisionResponse);

            // Act
            var result = await context.SystemUnderTest.GetSection(context.VisionUserSession, section);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordSectionResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetSection_Exception_ReturnsBadGateway()
        {
            var context = new VisionPatientRecordSectionServiceTestContext();
            context.VisionClient.Setup(x => x.GetPatientData(context.VisionUserSession,
                    It.Is<PatientDataRequest>(x => x.View == "TEST RESULTS")))
                .Throws<HttpRequestException>();

            // Act
            var result = await context.SystemUnderTest.GetSection(context.VisionUserSession, VisionMapperType.TestResults);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordSectionResult.BadGateway>();
        }
    }
}
