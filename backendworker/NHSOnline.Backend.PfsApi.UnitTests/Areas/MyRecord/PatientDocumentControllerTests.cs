using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.MyRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MyRecord
{
    [TestClass]
    public class PatientDocumentControllerTests
    {
        private PatientDocumentController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _userSession = _fixture.Create<UserSession>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };

            var httpContextResponse = new DefaultHttpResponse(new DefaultHttpContext());

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);
            httpContextMock.SetupGet(x => x.Response).Returns(httpContextResponse);

            _systemUnderTest = _fixture.Create<PatientDocumentController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task GetPatientDocument_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var documentResponse = new PatientDocument
            {
                Content = "test content",
                HasErrored = false,
            };
            var getDocumentResponse = new GetPatientDocumentResult.Success(documentResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetPatientDocument(_userSession.GpUserSession, 
                "1", "jpg", "example"))
                .Returns(Task.FromResult((GetPatientDocumentResult)getDocumentResponse));

            // Act
            var documentInfo = new DocumentInfo
            {
                Type = "jpg",
                Name = "example"
            };
            var result = await _systemUnderTest.GetPatientDocument("1", documentInfo);

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetPatientDocument(_userSession.GpUserSession, 
                "1", "jpg", "example"));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<GetPatientDocumentResult.Success>();
        }

        [TestMethod]
        public async Task GetPatientDocumentForDownload_ReturnsSuccessfulFile()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var documentInfo = new DocumentInfo
            {
                Type = "jpg",
                Name = "example"
            };
            var patientDocument = new PatientDocument
            {
                Content = "<html><body><img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=='</body></html>",
                HasErrored = false
            };

            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);
            
            patientRecordService.Setup(x => x.GetPatientDocumentForDownload(_userSession.GpUserSession, 
                    "1", "jpg", "example"))
                .Returns(Task.FromResult(patientDocument));
            var result = await _systemUnderTest.GetPatientDocumentForDownload("1", documentInfo);
            
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.GpUserSession.Supplier));
            patientRecordService.Verify(x => x.GetPatientDocumentForDownload(_userSession.GpUserSession, 
                "1", "jpg", "example"));
            result.Should().BeAssignableTo<FileContentResult>();
        }
    }
}