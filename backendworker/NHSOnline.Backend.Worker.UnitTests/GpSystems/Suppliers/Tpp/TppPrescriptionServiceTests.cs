using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class TppPrescriptionServiceTests
    {
        private TppPrescriptionService _systemUnderTest;
        private Mock<ITppClient> _tppClient;
        private Mock<ITppPrescriptionMapper> _tppPrescriptionMapper;
        private IOptions<ConfigurationSettings> _options;
        private TppUserSession _userSession;
        private IFixture _fixture;

        private const int PrescriptionsMaxCoursesSoftLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _tppPrescriptionMapper = _fixture.Freeze<Mock<ITppPrescriptionMapper>>();
            _userSession = _fixture.Freeze<TppUserSession>();
            _options = Options.Create(new ConfigurationSettings
            {
                PrescriptionsMaxCoursesSoftLimit = PrescriptionsMaxCoursesSoftLimit
            });
            _fixture.Inject(_options);
            _systemUnderTest = _fixture.Create<TppPrescriptionService>();
        }

        #region Get Prescriptions

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var prescriptionsResponse = _fixture.Create<ListRepeatMedicationReply>();

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<PrescriptionResult.SuccessfullGet>();
            ((PrescriptionResult.SuccessfullGet) result).Response.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromEmis(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var medications = new List<Medication>();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
            {
                medications.Add(new Medication
                {
                    DrugId = Guid.NewGuid().ToString(),
                    Drug = "Drug " + i,
                    Details = _fixture.Create<string>(),
                    Requestable = "y",
                    Type = "Repeat",
                });
            }

            var listRepeatMedicationReply = new ListRepeatMedicationReply()
            {
                Medications = medications
            };

            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession)).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>(HttpStatusCode.OK)
                    {
                        Body = listRepeatMedicationReply,
                    }));

            var response = new PrescriptionListResponse();
            List<Medication> capturedItemToMap = null;
            _tppPrescriptionMapper.Setup(x => x.Map(It.IsAny<List<Medication>>())).Returns(response)
                .Callback<List<Medication>>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_userSession);

            // Assert
            _tppClient.Verify(x => x.ListRepeatMedicationPost(_userSession));
            result.Should().BeAssignableTo<PrescriptionResult.SuccessfullGet>();
            ((PrescriptionResult.SuccessfullGet) result).Response.Should().NotBeNull();

            var getPrescriptionsResult = (PrescriptionResult.SuccessfullGet) result;
            getPrescriptionsResult.Response.Should().Be(response);

            capturedItemToMap.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavilable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<ListRepeatMedicationReply>
                            (HttpStatusCode.InternalServerError)
                            {
                                ErrorResponse = _fixture.Create<Error>()
                            }));
            // Act
            var result = await _systemUnderTest.GetPrescriptions(_userSession);

            // Assert
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            _tppClient.Setup(x => x.ListRepeatMedicationPost(_userSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(_userSession);

            // Assert
            result.Should().BeAssignableTo<PrescriptionResult.SupplierSystemUnavailable>();
            _tppClient.Verify();
        }

        #endregion
    }
}