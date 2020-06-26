using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.MyRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public sealed class MyRecordSectionControllerTests: IDisposable
    {
        private MyRecordSectionController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private P9UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new VisionUserSession(), "im1token");

            _systemUnderTest = new MyRecordSectionController(
                new Mock<ILogger<MyRecordSectionController>>().Object,
                _mockGpSystemFactory.Object,
                new Mock<IAuditor>().Object);
        }

        [DataTestMethod]
        [DataRow("TestResults", VisionMapperType.TestResults)]
        [DataRow("Diagnosis", VisionMapperType.Diagnosis)]
        [DataRow("Examinations", VisionMapperType.Examinations)]
        [DataRow("Procedures", VisionMapperType.Procedures)]
        public async Task GetSection_Successful(string section, VisionMapperType visionSection)
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IVisionPatientRecordService>();
            var response = new MyRecordSectionResponse();

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(
                    x => x.GetSection(_userSession.GpUserSession, visionSection))
                .ReturnsAsync(new GetMyRecordSectionResult.Success(response));

            // Act
            var result = await _systemUnderTest.GetSection(section, _userSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetMyRecordSectionResult.Success>();
        }

        [DataTestMethod]
        [DataRow("testResults")]
        [DataRow("Medications")]
        [DataRow("Allergies")]
        [DataRow("Immunisations")]
        [DataRow("Problems")]
        [DataRow("Other")]
        public async Task GetSection_InvalidSection_BadRequest(string section)
        {
            // Act
            var result = await _systemUnderTest.GetSection(section, _userSession);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task GetSection_Emis_BadRequest()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", userSession);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task GetSection_Tpp_BadRequest()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), gpUserSession: new TppUserSession(), "im1token");

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", userSession);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task GetSection_Microtest_BadRequest()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new MicrotestUserSession(), "im1token");

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", userSession);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task GetSection_BadGatewayFromService_ReturnsBadGateway()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IVisionPatientRecordService>();

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(
                    x => x.GetSection(_userSession.GpUserSession, VisionMapperType.TestResults))
                .ReturnsAsync(new GetMyRecordSectionResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetSection_InternalServerErrorFromService_ReturnsInternalServerError()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IVisionPatientRecordService>();

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(
                    x => x.GetSection(_userSession.GpUserSession, VisionMapperType.TestResults))
                .ReturnsAsync(new GetMyRecordSectionResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task GetSection_BadRequestFromService_ReturnsBadRequest()
        {
            // Arrange
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IVisionPatientRecordService>();

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(
                    x => x.GetSection(_userSession.GpUserSession, VisionMapperType.TestResults))
                .ReturnsAsync(new GetMyRecordSectionResult.BadRequest());

            // Act
            var result = await _systemUnderTest.GetSection("TestResults", _userSession);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}