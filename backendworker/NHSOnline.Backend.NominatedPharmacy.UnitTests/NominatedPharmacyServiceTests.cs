using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;
using static NHSOnline.Backend.NominatedPharmacy.Soap.GetNominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyServiceTests
    {
        private NominatedPharmacyService _systemUnderTest;

        private Mock<INominatedPharmacyClient> _nominatedPharmacyClient;
        private Mock<INominatedPharmacyConfigurationSettings> _configMock;
        private IFixture _fixture;

        private const string SpineAccreditedSystemIdFrom = "0001";
        private const string SpineAccreditedSystemIdTo = "0002";
        private const string NhsNumber = "239 372 9384";
        private const string NhsNumberTrimmed = "2393729384";
        private const string CurrentPharmacyOdsCode = "PHA12";
        private const string UpdatedPharmacyOdsCode = "AB837";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nominatedPharmacyClient = _fixture.Freeze<Mock<INominatedPharmacyClient>>();
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfigurationSettings>>();
            _configMock.SetupGet(x => x.SpineAccreditedSystemIdFrom).Returns(SpineAccreditedSystemIdFrom);
            _configMock.SetupGet(x => x.SpineAccreditedSystemIdTo).Returns(SpineAccreditedSystemIdTo);

            _systemUnderTest = _fixture.Create<NominatedPharmacyService>();
        }

        [TestMethod]
        public async Task
            NominatedPharmacyGet_ReturnsSuccessfulWhenRequestSucceeds_ReturnsNullCodeAndValidIfThereAreNoFoundPharmacyNominations()
        {
            // Arrange
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.Is<QUPAIN000008UK02>(
                        req => req.CommunicationFunctionRcv.Device.Id.Extension.Equals(SpineAccreditedSystemIdTo,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.CommunicationFunctionSnd.Device.Id.Extension.Equals(SpineAccreditedSystemIdFrom,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.ControlActEvent.Query.PersonId.Value.Extension.Equals(NhsNumberTrimmed,
                                   StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>(HttpStatusCode.OK)
                    {
                        RawResponse = new Soap.NominatedPharmacyResponseEnvelope<QUPAIN000009UK03Response>
                        {
                            Body = new Body<QUPAIN000009UK03Response>
                            {
                                RetrievalQueryResponse = new QUPAIN000009UK03Response
                                {
                                    QUPAIN000009UK03 = new QUPAIN000009UK03
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
                                                                PlayedOtherProviderPatients =
                                                                    new List<PlayedOtherProviderPatient>()
                                                            }
                                                        }
                                                    },
                                                    PertinentInformation = new PertinentInformation
                                                    {
                                                        PertinentSerialChangeNumber = new PertinentSerialChangeNumber
                                                        {
                                                            Value = new ValueElement
                                                            {
                                                                Value = "22",
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.PharmacyOdsCode.Should().Be(null);
            result.HasValidPharmacyType.Should().Be(true);
        }

        [DataTestMethod]
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, true)]
        [DataRow("P2", HttpStatusCode.OK, null, false)]
        [DataRow("P3", HttpStatusCode.OK, CurrentPharmacyOdsCode, true)]
        [DataRow("1", HttpStatusCode.OK, null, true)] // we're only looking at p1, p2, p3
        public async Task
            NominatedPharmacyGet_ReturnsSuccessfulWhenRequestSucceeds_ReturnsOdsCodeIfValidAndIfItIsAValidPharmacyType(
                string code,
                HttpStatusCode httpStatusCode,
                string expectedPharmacyOdsCodeInResult,
                bool hasValidPharmacyType)
        {
            // Arrange
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.Is<QUPAIN000008UK02>(
                        req => req.CommunicationFunctionRcv.Device.Id.Extension.Equals(SpineAccreditedSystemIdTo,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.CommunicationFunctionSnd.Device.Id.Extension.Equals(SpineAccreditedSystemIdFrom,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.ControlActEvent.Query.PersonId.Value.Extension.Equals(NhsNumberTrimmed,
                                   StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(
                    new NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>(HttpStatusCode.OK)
                    {
                        RawResponse = new Soap.NominatedPharmacyResponseEnvelope<QUPAIN000009UK03Response>
                        {
                            Body = new Body<QUPAIN000009UK03Response>
                            {
                                RetrievalQueryResponse = new QUPAIN000009UK03Response
                                {
                                    QUPAIN000009UK03 = new QUPAIN000009UK03
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
                                                                PlayedOtherProviderPatients =
                                                                    new List<PlayedOtherProviderPatient>
                                                                    {
                                                                        new PlayedOtherProviderPatient
                                                                        {
                                                                            SubjectOf = new SubjectOf
                                                                            {
                                                                                PatientCareProvisionEvent =
                                                                                    new PatientCareProvisionEvent
                                                                                    {
                                                                                        Code = new CodeElement
                                                                                        {
                                                                                            Code = code
                                                                                        },
                                                                                        Performer = new Performer
                                                                                        {
                                                                                            AssignedEntity =
                                                                                                new AssignedEntity
                                                                                                {
                                                                                                    Id = new Id
                                                                                                    {
                                                                                                        Extension =
                                                                                                            CurrentPharmacyOdsCode,
                                                                                                    }
                                                                                                }
                                                                                        }
                                                                                    }
                                                                            }
                                                                        }
                                                                    }
                                                            }
                                                        }
                                                    },
                                                    PertinentInformation = new PertinentInformation
                                                    {
                                                        PertinentSerialChangeNumber = new PertinentSerialChangeNumber
                                                        {
                                                            Value = new ValueElement
                                                            {
                                                                Value = "22",
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(httpStatusCode);
            result.PharmacyOdsCode.Should().Be(expectedPharmacyOdsCodeInResult);
            result.HasValidPharmacyType.Should().Be(hasValidPharmacyType);
        }

        [TestMethod]
        public async Task NominatedPharmacyUpdate_Returns400_WhenUpdateClientFails()
        {
            // Arrange
            string pertinentSerialChangeNumber = new Guid().ToString();

            _nominatedPharmacyClient
                .Setup(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdateRequest>()))
                .Returns(Task.FromResult(
                    new UpdateNominatedPharmacyApiObjectResponse(HttpStatusCode.BadRequest)
                ))
                .Verifiable();

            var nominatedPharmacyUpdate = new NominatedPharmacyUpdate
            {
                HasExistingNominatedPharmacy = true,
                UpdatedOdsCode = UpdatedPharmacyOdsCode,
                NhsNumber = NhsNumber,
                PertinentSerialChangeNumber = pertinentSerialChangeNumber,
            };

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nominatedPharmacyUpdate);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task NominatedPharmacyUpdate_ReturnsSuccessfulResponse_WhenClientUpdateIsSuccessful()
        {
            // Arrange
            string pertinentSerialChangeNumber = new Guid().ToString();

            _nominatedPharmacyClient
                .Setup(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdateRequest>()))
                .Returns(Task.FromResult(
                    new UpdateNominatedPharmacyApiObjectResponse(HttpStatusCode.OK)
                ))
                .Verifiable();

            var nominatedPharmacyUpdate = new NominatedPharmacyUpdate
            {
                HasExistingNominatedPharmacy = true,
                UpdatedOdsCode = UpdatedPharmacyOdsCode,
                NhsNumber = NhsNumber,
                PertinentSerialChangeNumber = pertinentSerialChangeNumber,
            };

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nominatedPharmacyUpdate);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task NominatedPharmacyUpdate_ReturnsInternalServerError_WhenClientThrowsException()
        {
            // Arrange
            string pertinentSerialChangeNumber = new Guid().ToString();

            _nominatedPharmacyClient
                .Setup(x => x.UpdateNominatedPharmacy(It.IsAny<NominatedPharmacyUpdateRequest>()))
                .Throws<Exception>()
                .Verifiable();

            var nominatedPharmacyUpdate = new NominatedPharmacyUpdate
            {
                HasExistingNominatedPharmacy = true,
                UpdatedOdsCode = UpdatedPharmacyOdsCode,
                NhsNumber = NhsNumber,
                PertinentSerialChangeNumber = pertinentSerialChangeNumber,
            };

            // Act
            var result = await _systemUnderTest.UpdateNominatedPharmacy(nominatedPharmacyUpdate);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [DataTestMethod]
        [DataRow(new[] { "P1", "P2" }, HttpStatusCode.OK)]
        [DataRow(new[] { "P1", "P3" }, HttpStatusCode.OK)]
        [DataRow(new[] { "P1", "P2", "P3", "1"}, HttpStatusCode.OK)]
        [DataRow(new[] { "P2", "P3" }, HttpStatusCode.OK)]
        public async Task
            NominatedPharmacyGet_ReturnsSuccessfulWithNullOdsCodeAndFalseHasValidPharmacyType_WhenMultiplePharmacyTypesExist(
                string[] pharmacyCodes,
                HttpStatusCode httpStatusCode)
        {
            // Arrange
            const string ExpectedPharmacyOdsCodeInResult = null;
            
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.Is<QUPAIN000008UK02>(
                        req => req.CommunicationFunctionRcv.Device.Id.Extension.Equals(SpineAccreditedSystemIdTo,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.CommunicationFunctionSnd.Device.Id.Extension.Equals(SpineAccreditedSystemIdFrom,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.ControlActEvent.Query.PersonId.Value.Extension.Equals(NhsNumberTrimmed,
                                   StringComparison.OrdinalIgnoreCase))
                ))
                .Returns(Task.FromResult(GetNominatedPharmacyApiObjectResponse(pharmacyCodes)))               
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(httpStatusCode);
            result.PharmacyOdsCode.Should().Be(ExpectedPharmacyOdsCodeInResult);
            result.HasValidPharmacyType.Should().Be(false);
        }
        
        [DataTestMethod]
        public async Task
            NominatedPharmacyGet_ReturnsUnsuccessfulResponseWhenPharmacySearchFailsDueToClientFailure()
        {
            //Arrange
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.IsAny<QUPAIN000008UK02>()
                )).Returns(Task.FromResult(GetUnsuccessfulNominatedPharmacyApiObjectResponse()))               
                .Verifiable();

            //Act
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber);
            
            //Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            result.HasValidPharmacyType.Should().Be(false);        
        }
        
        [TestMethod]
        public async Task NominatedPharmacyGet_ReturnsInternalServerError_WhenClientThrowsException()
        {
            // Arrange
            string NhSNumber = "ABC123";
            
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.IsAny<QUPAIN000008UK02>()
                )).Throws<Exception>()         
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetNominatedPharmacy(NhSNumber);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        private static NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>
            GetUnsuccessfulNominatedPharmacyApiObjectResponse()
        {
            return new NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>(HttpStatusCode.ServiceUnavailable)
            {
                RawResponse = new Soap.NominatedPharmacyResponseEnvelope<QUPAIN000009UK03Response>
                {
                    Body = null
                }
            };
        }
           
        private static NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>
            GetNominatedPharmacyApiObjectResponse(string[] pharmacyCodes)
        {
            return new NominatedPharmacyApiObjectResponse<QUPAIN000009UK03Response>(HttpStatusCode.OK)
            {
                RawResponse = new Soap.NominatedPharmacyResponseEnvelope<QUPAIN000009UK03Response>
                {
                    Body = new Body<QUPAIN000009UK03Response>
                    {
                        RetrievalQueryResponse = new QUPAIN000009UK03Response
                        {
                            QUPAIN000009UK03 = new QUPAIN000009UK03
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
                                                        PlayedOtherProviderPatients =
                                                            GetPatientCareProvisionEvents(pharmacyCodes)
                                                    }
                                                }
                                            },
                                            PertinentInformation = new PertinentInformation
                                            {
                                                PertinentSerialChangeNumber = new PertinentSerialChangeNumber
                                                {
                                                    Value = new ValueElement
                                                    {
                                                        Value = "22",
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
            };
        }
        
        private static List<PlayedOtherProviderPatient> GetPatientCareProvisionEvents(string[] pharmacyCodes)
        {
            var playedOtherProviderPatientList = new List<PlayedOtherProviderPatient>();
            
            foreach (var pharmacyCode in pharmacyCodes)
            {
                if (!pharmacyCode.IsNullOrEmpty())
                {
                    playedOtherProviderPatientList.Add(new PlayedOtherProviderPatient
                        {
                            SubjectOf = new SubjectOf
                            {
                                PatientCareProvisionEvent =
                                    new PatientCareProvisionEvent
                                    {
                                        Code = new CodeElement
                                        {
                                            Code = pharmacyCode
                                        },
                                        Performer = new Performer
                                        {
                                            AssignedEntity = new AssignedEntity
                                            {
                                                Id = new Id
                                                {
                                                    Extension =
                                                        CurrentPharmacyOdsCode,
                                                }
                                            }
                                        }
                                    }
                            }
                        }
                    );
                }
            }

            return playedOtherProviderPatientList;
        }
    }
}