using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using Moq;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _userSession = _fixture.Freeze<TppUserSession>();
            _systemUnderTest = _fixture.Create<TppPatientRecordService>();
        }
        
        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
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

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _tppClient.Verify(x => x.PatientOverviewPost(It.IsAny<TppUserSession>()));
            result.Should().BeAssignableTo<GetMyRecordResult.SuccessfullyRetrieved>();
            ((GetMyRecordResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }
        
        private List<ViewPatientOverViewItem> CreateListPatientOverviewItem(int count)
        {
            var result = new List<ViewPatientOverViewItem>();
            for (int i = 0; i < count; i++)
            {
                result.Add(CreatePatientOverviewItem());
            }
            return result;
        }
        
        private ViewPatientOverViewItem CreatePatientOverviewItem()
        {
            return new ViewPatientOverViewItem
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(),
                Value = _fixture.Create<string>(),
            };
        }
        
        private Event CreateEvent()
        {
            return new Event
            {
                DoneBy = _fixture.Create<string>(),
                Location = _fixture.Create<string>(),
                Date = _fixture.Create<DateTimeOffset>().ToString(),
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
    }
}