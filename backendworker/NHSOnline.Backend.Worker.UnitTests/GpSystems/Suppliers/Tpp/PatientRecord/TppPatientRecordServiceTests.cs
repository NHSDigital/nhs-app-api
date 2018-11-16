using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using Moq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppPatientRecordServiceTests
    {
        private TppPatientRecordService _systemUnderTest;
        private Mock<ITppClient> _tppClient;
        private TppUserSession _userSession;
        private IFixture _fixture;
        private Mock<IGetPatientDcrEventsTaskChecker> _patientDcrEventsChecker;
        private Mock<IGetPatientOverviewTaskChecker> _patientOverviewTaskChecker;
        private Mock<IGetPatientTestResultsTaskChecker> _patientTestResultsChecker;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _userSession = _fixture.Freeze<TppUserSession>();
            _patientDcrEventsChecker = _fixture.Freeze<Mock<IGetPatientDcrEventsTaskChecker>>();
            _patientOverviewTaskChecker = _fixture.Freeze<Mock<IGetPatientOverviewTaskChecker>>();
            _patientTestResultsChecker = _fixture.Freeze<Mock<IGetPatientTestResultsTaskChecker>>();
            _systemUnderTest = _fixture.Create<TppPatientRecordService>();
        }
        
        [TestMethod]
        public async Task GetMyRecord_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var tppAllergies = CreateListPatientOverviewItem(1);
            var tppDrugSensitivities = CreateListPatientOverviewItem(2);
            var tppAcuteMedications = CreateListPatientOverviewItem(3);
            var tppCurrentRepeatMedications = CreateListPatientOverviewItem(4);
            var tppPastRepeatMedications = CreateListPatientOverviewItem(5);
            
            var patientOverviewResponse = new ViewPatientOverviewReply
            {
                Allergies = tppAllergies,
                DrugSensitivities = tppDrugSensitivities,
                Drugs = tppAcuteMedications,
                CurrentRepeats = tppCurrentRepeatMedications,
                PastRepeats = tppPastRepeatMedications,
            };

            var patientRecordResponse = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    CreateEvent()
                }
            };

            var testResultsResponse = new TestResultsViewReply
            {
                Items = new List<TestResultsViewReplyItem>
                {
                    CreateTestResultsViewReplyItem()
                }
            };
            
            _tppClient.Setup(x => x.PatientOverviewPost(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ViewPatientOverviewReply>(HttpStatusCode.OK)
                    {
                        Body = patientOverviewResponse,
                        ErrorResponse = null,
                    }));   
            
            _tppClient.Setup(x => x.RequestPatientRecordPost(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<RequestPatientRecordReply>(HttpStatusCode.OK)
                    {
                        Body = patientRecordResponse,
                        ErrorResponse = null,
                    }));  
            
            _tppClient.Setup(x => x.TestResultsView(It.IsAny<TppUserSession>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<TestResultsViewReply>(HttpStatusCode.OK)
                    {
                        Body = testResultsResponse,
                        ErrorResponse = null,
                    }));

            _patientDcrEventsChecker.Setup(x =>
                    x.Check(It.IsAny<TppClient.TppApiObjectResponse<RequestPatientRecordReply>>()))
                .Returns(Mock.Of<TppDcrEvents>())
                .Verifiable();
            
            _patientOverviewTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppClient.TppApiObjectResponse<ViewPatientOverviewReply>>()))
                .Returns(new Tuple<Allergies, Medications>(Mock.Of<Allergies>(), Mock.Of<Medications>()))
                .Verifiable();
            
            _patientTestResultsChecker.Setup(x =>
                    x.Check(It.IsAny<TppClient.TppApiObjectResponse<TestResultsViewReply>>()))
                .Returns(Mock.Of<TestResults>())
                .Verifiable();
            
           // Act
            var result = await _systemUnderTest.GetMyRecord(_userSession);

            // Assert
            _tppClient.Verify(x => x.PatientOverviewPost(It.IsAny<TppUserSession>()));
            _patientDcrEventsChecker.Verify();
            _patientOverviewTaskChecker.Verify();
            _patientTestResultsChecker.Verify();
            result.Should().BeAssignableTo<GetMyRecordResult.SuccessfullyRetrieved>();
            ((GetMyRecordResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }
        
        private List<ViewPatientOverViewItem> CreateListPatientOverviewItem(int count)
        {
            var result = new List<ViewPatientOverViewItem>();
            for (var i = 0; i < count; i++)
            {
                result.Add(CreatePatientOverviewItem());
            }
            return result;
        }
        
        private ViewPatientOverViewItem CreatePatientOverviewItem()
        {
            return new ViewPatientOverViewItem
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Value = _fixture.Create<string>(),
            };
        }
        
        private Event CreateEvent()
        {
            return new Event
            {
                DoneBy = _fixture.Create<string>(),
                Location = _fixture.Create<string>(),
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Items = new List<RequestPatientRecordItem>
                {            
                    new RequestPatientRecordItem
                    {
                        Details =_fixture.Create<string>(),
                        Type = _fixture.Create<string>()
                    }
                    
                }
            };
        }
        
        private TestResultsViewReplyItem CreateTestResultsViewReplyItem()
        {
            return new TestResultsViewReplyItem
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(CultureInfo.InvariantCulture),
                Value = _fixture.Create<string>(),
                Description = _fixture.Create<string>()
            };
        }
        
    }
}
