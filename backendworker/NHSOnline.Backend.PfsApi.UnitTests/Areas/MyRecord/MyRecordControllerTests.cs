using System;
using System.Threading.Tasks;
using FluentAssertions;
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
using GetMyRecordResult = NHSOnline.Backend.GpSystems.PatientRecord.GetMyRecordResult;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public sealed class MyRecordControllerTests: IDisposable
    {
        private MyRecordController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private P9UserSession _userSession;
        private Guid _patientGuid;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientGuid = Guid.NewGuid();
            
            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _systemUnderTest = new MyRecordController(
                new Mock<ILogger<MyRecordController>>().Object,
                _mockGpSystemFactory.Object,
                new Mock<IAuditor>().Object,
                new Mock<IMyRecordMetadataLogger>().Object);
        }
        
        [TestMethod]
        public async Task GetAllergies_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var allergyRequestResponse = new MyRecordResponse();
            var getAllergiesResponse = new GetMyRecordResult.Success(allergyRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((GetMyRecordResult)getAllergiesResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord(_patientGuid, _userSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(
                It.Is<GpLinkedAccountModel>(
                    m => m.GpUserSession == _userSession.GpUserSession && m.PatientId == _patientGuid)));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetMyRecordResult.Success>();
        }
        
        [TestMethod]
        public async Task GetMedications_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var medicationRequestResponse = new MyRecordResponse();
            var getMedicationsResponse = new GetMyRecordResult.Success(medicationRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((GetMyRecordResult)getMedicationsResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord(_patientGuid, _userSession);
            

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetMyRecordResult.Success>();
        }
        
        [TestMethod]
        public async Task GetProblems_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var problemRequestResponse = new MyRecordResponse();
            var getProblemsRequestResponse = new GetMyRecordResult.Success(problemRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((GetMyRecordResult)getProblemsRequestResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord(_patientGuid, _userSession);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientGuid)));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetMyRecordResult.Success>();
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}