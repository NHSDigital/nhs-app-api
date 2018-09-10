using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.MyRecord;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using GetMyRecordResult = NHSOnline.Backend.Worker.GpSystems.PatientRecord.GetMyRecordResult;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.MyRecord
{
    [TestClass]
    public class MyRecordControllerTests
    {
        private MyRecordController _systemUnderTest;
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

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<MyRecordController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }
        
        [TestMethod]
        public async Task GetAllergies_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var allergyRequestResponse = new MyRecordResponse();
            var getAllergiesResponse = new GetMyRecordResult.SuccessfullyRetrieved(allergyRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(_userSession)).Returns(Task.FromResult((GetMyRecordResult)getAllergiesResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(_userSession));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetMyRecordResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }
        
        [TestMethod]
        public async Task GetMedications_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var medicationRequestResponse = new MyRecordResponse();
            var getMedicationsResponse = new GetMyRecordResult.SuccessfullyRetrieved(medicationRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(_userSession)).Returns(Task.FromResult((GetMyRecordResult)getMedicationsResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(_userSession));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetMyRecordResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }
        
        [TestMethod]
        public async Task GetProblems_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            var mockGpSystem = new Mock<IGpSystem>();
            var patientRecordService = new Mock<IPatientRecordService>();
            var problemRequestResponse = new MyRecordResponse();
            var getProblemsRequestResponse = new GetMyRecordResult.SuccessfullyRetrieved(problemRequestResponse);
            
            // Arrange
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(_userSession.Supplier))
                .Returns(mockGpSystem.Object);

            mockGpSystem.Setup(x => x.GetPatientRecordService())
                .Returns(patientRecordService.Object);

            patientRecordService.Setup(x => x.GetMyRecord(_userSession)).Returns(Task.FromResult((GetMyRecordResult)getProblemsRequestResponse));

            // Act
            var result = await _systemUnderTest.GetMyRecord();

            // Assert
            _mockGpSystemFactory.Verify(x => x.CreateGpSystem(_userSession.Supplier));
            mockGpSystem.Verify(x => x.GetPatientRecordService());
            patientRecordService.Verify(x => x.GetMyRecord(_userSession));
            var okObjectResult = result as OkObjectResult;
            Assert.IsNotNull(okObjectResult);
            var value = okObjectResult.Value as GetMyRecordResult.SuccessfullyRetrieved;
            Assert.IsNotNull(value);
        }
    }
}