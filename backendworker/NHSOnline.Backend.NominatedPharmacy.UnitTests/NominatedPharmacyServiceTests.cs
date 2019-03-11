using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static NHSOnline.Backend.NominatedPharmacy.NominatedPharmacyClient;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyServiceTests
    {
        private NominatedPharmacyService _systemUnderTest;

        private Mock<INominatedPharmacyClient> _nominatedPharmacyClient;
        private Mock<INominatedPharmacyConfig> _configMock;
        private IFixture _fixture;

        const string SpineAccreditedSystemIdFrom = "0001";
        const string SpineAccreditedSystemIdTo = "0002";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nominatedPharmacyClient = _fixture.Freeze<Mock<INominatedPharmacyClient>>();
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfig>>();
            _configMock.SetupGet(x => x.SpineAccreditedSystemIdFrom).Returns(SpineAccreditedSystemIdFrom);
            _configMock.SetupGet(x => x.SpineAccreditedSystemIdTo).Returns(SpineAccreditedSystemIdTo);
            _configMock.SetupGet(x => x.MessageId).Returns(new Guid().ToString());
            _configMock.SetupGet(x => x.SdsRoleId).Returns("roleId");
            _configMock.SetupGet(x => x.SdsUserId).Returns("userId");
            _configMock.SetupGet(x => x.SdsRole).Returns("sdsRole");

            _systemUnderTest = _fixture.Create<NominatedPharmacyService>();
        }

        [DataTestMethod]
        [DataRow("P1", HttpStatusCode.OK, "AB837")]
        [DataRow("P2", HttpStatusCode.OK, null)]
        [DataRow("P3", HttpStatusCode.OK, null)]
        [DataRow("1", HttpStatusCode.OK, null)]
        public async Task NominatedPharmacyGet_ReturnsSuccessfulResponseWithPharmacyOdsCode_WhenOdsCodeWhenCodeIsP1AndNotOtherwise(
            string code,
            HttpStatusCode httpStatusCode,
            string expectedPharmacyOdsCodeInResult)
        {
            // Arrange
            const string nhsNumber = "2393729384";
            const string pharmacyOdsCode = "AB837";

            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.Is<QUPA_IN000008UK02>(
                        req => req.ControlActEvent.Author.AgentPersonSDS.Id.Extension.Equals("roleId", StringComparison.OrdinalIgnoreCase) &&
                        req.CommunicationFunctionRcv.Device.Id.Extension.Equals(SpineAccreditedSystemIdTo, StringComparison.OrdinalIgnoreCase) &&
                        req.CommunicationFunctionSnd.Device.Id.Extension.Equals(SpineAccreditedSystemIdFrom,  StringComparison.OrdinalIgnoreCase))
                    ))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>(HttpStatusCode.OK)
                    {
                        RawResponse = new Soap.NominatedPharmacyResponseEnvelope<QUPA_IN000009UK03_Response>
                        {
                            Body = new Body<QUPA_IN000009UK03_Response>
                            {
                                RetrievalQueryResponse = new QUPA_IN000009UK03_Response
                                {
                                    QUPA_IN000009UK03 = new QUPA_IN000009UK03
                                    {
                                        ControlActEvent = new ControlActEvent
                                        {
                                            Subject = new Subject
                                            {
                                                PDSResponse = new PDSResponse
                                                {
                                                    Subject = new Subject
                                                    {
                                                        PatientRole = new PatientRole
                                                        {
                                                            PatientPerson = new PatientPerson
                                                            {
                                                                PlayedOtherProviderPatient = new PlayedOtherProviderPatient
                                                                {
                                                                    SubjectOf = new SubjectOf
                                                                    {
                                                                        PatientCareProvisionEvent = new PatientCareProvisionEvent
                                                                        {
                                                                            Code = new Code
                                                                            {
                                                                                _code = code
                                                                            },
                                                                            Performer = new Performer
                                                                            {
                                                                                AssignedEntity = new AssignedEntity
                                                                                {
                                                                                    Id = new Id
                                                                                    {
                                                                                        Extension = pharmacyOdsCode,
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }))
                    .Verifiable();

            // Act
            var result = await _systemUnderTest.GetNominatedPharmacy(nhsNumber);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(httpStatusCode);
            result.PharmacyOdsCode.Should().Be(expectedPharmacyOdsCodeInResult);
        }
    }
}
