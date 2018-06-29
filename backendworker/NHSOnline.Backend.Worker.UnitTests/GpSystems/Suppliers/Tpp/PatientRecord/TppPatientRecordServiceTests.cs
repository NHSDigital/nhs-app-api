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
            
            _tppClient.Setup(x => x.PatientOverviewPost(It.IsAny<ViewPatientOverview>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ViewPatientOverviewReply>(HttpStatusCode.OK)
                    {
                        Body = patientOverviewResponse,
                        ErrorResponse = null,
                    }));            

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _tppClient.Verify(x => x.PatientOverviewPost(It.IsAny<ViewPatientOverview>(), It.IsAny<string>()));
            result.Should().BeAssignableTo<GetMyRecordResult.SuccessfullyRetrieved>();
            ((GetMyRecordResult.SuccessfullyRetrieved)result).Response.Should().NotBeNull();
        }
        
        private List<Item> CreateListPatientOverviewItem(int count)
        {
            var result = new List<Item>();
            for (int i = 0; i < count; i++)
            {
                result.Add(CreatePatientOverviewItem());
            }
            return result;
        }
        
        private Item CreatePatientOverviewItem()
        {
            return new Item
            {
                Date = _fixture.Create<DateTimeOffset>().ToString(),
                Value = _fixture.Create<string>(),
            };
        }
    }
}