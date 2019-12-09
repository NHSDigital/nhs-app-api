using System;
using System.Collections.Generic;
using System.Globalization;
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
using NHSOnline.Backend.Support;
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
        private UserSession _userSession;

        private const string SpineAccreditedSystemIdFrom = "0001";
        private const string SpineAccreditedSystemIdTo = "0002";

        private const string NhsNumber = "239 372 9384";
        private const string NhsNumberTrimmed = "2393729384";
        private const string RandomNhsNumber = "1234567890";
        private const string CurrentPharmacyOdsCode = "PHA12";
        private const string UpdatedPharmacyOdsCode = "AB837";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _nominatedPharmacyClient = _fixture.Freeze<Mock<INominatedPharmacyClient>>();
            _configMock = _fixture.Freeze<Mock<INominatedPharmacyConfigurationSettings>>();
            var pdsTraceConfigurationSettings = new PdsTraceConfigurationSettings
            {
                FromAsid = SpineAccreditedSystemIdFrom,
                ToAsid = SpineAccreditedSystemIdTo,
            };
            _configMock.SetupGet(x => x.PdsTraceConfigurationSettings).Returns(pdsTraceConfigurationSettings);

            _userSession = _fixture.Create<UserSession>();

            _systemUnderTest = _fixture.Create<NominatedPharmacyService>();
        }

        [TestMethod]
        public async Task
            NominatedPharmacyGet_ReturnsSuccessfulWhenRequestSucceeds_ReturnsNullCodeAndValidIfThereAreNoFoundPharmacyNominations()
        {
            // Arrange
            _userSession.CitizenIdUserSession.FamilyName = "Smith";
            _userSession.CitizenIdUserSession.DateOfBirth = DateTime.ParseExact("19900101", "yyyyMMdd", CultureInfo.InvariantCulture);
 
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.Is<QUPAIN000008UK02>(
                        req => req.CommunicationFunctionRcv.Device.Id.Extension.Equals(SpineAccreditedSystemIdTo,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.CommunicationFunctionSnd.Device.Id.Extension.Equals(SpineAccreditedSystemIdFrom,
                                   StringComparison.OrdinalIgnoreCase) &&
                               req.ControlActEvent.Query.PersonId.Value.Extension.Equals(NhsNumberTrimmed,
                                   StringComparison.OrdinalIgnoreCase) && req.ControlActEvent.Query.RetrievalItems.Count == 4 &&
                                   req.ControlActEvent.Query.RetrievalItems[0].SemanticsText.Equals("person.nameUsual", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[1].SemanticsText.Equals("person.otherDemographics", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[2].SemanticsText.Equals("pharmacy", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[3].SemanticsText.Equals("person.confidentiality", StringComparison.Ordinal))
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
                                                                BirthTime = new BirthTime
                                                                {
                                                                    Value = "19900101"
                                                                },
                                                                PlayedOtherProviderPatients =
                                                                    new List<PlayedOtherProviderPatient>(),
                                                                
                                                                COCTMT000203UK02PartOfWhole = new COCTMT000203UK02PartOfWhole
                                                                {
                                                                    PartPerson = new PartPerson
                                                                    {
                                                                        Name = new Name
                                                                        {
                                                                            Family = "Smith",
                                                                            ValidTime = new ValidTime
                                                                            {
                                                                                Low = new Low
                                                                                {
                                                                                    Value = "20000101"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }, 
                                                            Id = new Id
                                                            {
                                                                Extension = NhsNumberTrimmed
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.PharmacyOdsCode.Should().Be(null);
            result.HaveAllChecksPassed.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, "Smith", "20000101", true)]
        [DataRow("P2", HttpStatusCode.OK, null, "Smith", "20000101", false)]
        [DataRow("P3", HttpStatusCode.OK, null, "Smith", "20000101", false)]
        [DataRow("1", HttpStatusCode.OK, null, "Smith", "20000101", true)] // we're only looking at p1, p2, p3
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, "", "20000101", false)] //no family name found
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, null, "20000101", false)] //no family name found
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, "Smith", "", false)] //no date of birth found
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, "Smith", null, false)] //no date of birth found
        [DataRow("P1", HttpStatusCode.OK, CurrentPharmacyOdsCode, "Smith", "badDate", false)] //date of birth unparsable
        public async Task
            NominatedPharmacyGet_ReturnsSuccessfulWhenRequestSucceeds_ReturnsOdsCodeIfValidAndIfItIsAValidPharmacyType(
                string code,
                HttpStatusCode httpStatusCode,
                string expectedPharmacyOdsCodeInResult,
                string surname,
                string dateOfBirth,
                bool expectedIsNominatedPharmacyEnabledValue)
        {
            // Arrange
            _userSession.CitizenIdUserSession.FamilyName = surname;

            string[] formats = { "yyyyMMdd" };
            bool parsedSuccessfully = DateTime.TryParseExact(dateOfBirth, formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDob);            
            
            if (parsedSuccessfully)
            {
                _userSession.CitizenIdUserSession.DateOfBirth = parsedDob;
            }

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
                                                                BirthTime = new BirthTime
                                                                {
                                                                    Value = dateOfBirth
                                                                },
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
                                                                    },
                                                                                                                                
                                                                COCTMT000203UK02PartOfWhole = new COCTMT000203UK02PartOfWhole
                                                                {
                                                                    PartPerson = new PartPerson
                                                                    {
                                                                        Name = new Name
                                                                        {
                                                                            Family = surname,
                                                                            ValidTime = new ValidTime
                                                                            {
                                                                                Low = new Low
                                                                                {
                                                                                    Value = "20000101"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    }
                                                            },
                                                            Id = new Id
                                                            {
                                                                Extension = NhsNumberTrimmed
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(httpStatusCode);
            result.PharmacyOdsCode.Should().Be(expectedPharmacyOdsCodeInResult);
            result.HaveAllChecksPassed.Should().Be(expectedIsNominatedPharmacyEnabledValue);
        }
        
        [DataTestMethod]
        [DataRow("Smith", "Smith", "20000101", "20000101", NhsNumberTrimmed, true)]
        [DataRow("Smith", "SMITH", "20000101", "20000101", NhsNumberTrimmed, true)]
        [DataRow("Smith", "Jones", "20000101", "20000101", NhsNumberTrimmed, false)] // family name does not match
        [DataRow("Smith", "Smith", "20000101", "20000102", NhsNumberTrimmed, false)] // date of birth does not match
        [DataRow("Smith", "Smith", "20000101", "20000101", RandomNhsNumber,  false)] // nhs number does not match    
        [DataRow("Smith",  null,   "20000101", "20000101", NhsNumberTrimmed, false)] // family name is not returned
        [DataRow("Smith", "Smith", "20000101",  null,      NhsNumberTrimmed, false)] // date of birth is not returned
        [DataRow("Smith", "Smith", "20000101", "20000101", null,             false)] // nhs number is not returned
        public async Task NominatedPharmacyGet_ShouldSetNominatedPharmacyNotEnabledIfAnyPersonalChecksFail(
                string expectedSurname,
                string returnedSurname,
                string expectedDateOfBirth,
                string returnedDateOfBirth,
                string expectedNhsNumber,
                bool expectedIsNominatedPharmacyEnabledValue)
        {
            // Arrange
            _userSession.CitizenIdUserSession.FamilyName = expectedSurname;

            string[] formats = { "yyyyMMdd" };
            bool parsedSuccessfully = DateTime.TryParseExact(expectedDateOfBirth, formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var expectedParsedDateOfBirth);

            if (parsedSuccessfully)
            {
                _userSession.CitizenIdUserSession.DateOfBirth = expectedParsedDateOfBirth;
            }

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
                                                                BirthTime = new BirthTime
                                                                {
                                                                    Value = returnedDateOfBirth
                                                                },
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
                                                                                            Code = "P1"
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
                                                                    },
                                                                                                                                
                                                                COCTMT000203UK02PartOfWhole = new COCTMT000203UK02PartOfWhole
                                                                {
                                                                    PartPerson = new PartPerson
                                                                    {
                                                                        Name = new Name
                                                                        {
                                                                            Family = returnedSurname,
                                                                            ValidTime = new ValidTime
                                                                            {
                                                                                Low = new Low
                                                                                {
                                                                                    Value = "20000101"
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            },
                                                            Id = new Id
                                                            {
                                                                Extension = expectedNhsNumber
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.HaveAllChecksPassed.Should().Be(expectedIsNominatedPharmacyEnabledValue);
        }

        

        [TestMethod]
        public async Task NominatedPharmacyGet_ReturnsFalseWhenNhsNumberReturnedDoesNotMatch()
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
                                   StringComparison.OrdinalIgnoreCase) && req.ControlActEvent.Query.RetrievalItems.Count == 4 &&
                                   req.ControlActEvent.Query.RetrievalItems[0].SemanticsText.Equals("person.nameUsual", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[1].SemanticsText.Equals("person.otherDemographics", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[2].SemanticsText.Equals("pharmacy", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[3].SemanticsText.Equals("person.confidentiality", StringComparison.Ordinal))
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
                                                            }, 
                                                            Id = new Id
                                                            {
                                                                Extension = RandomNhsNumber
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.PharmacyOdsCode.Should().Be(null);
            result.HaveAllChecksPassed.Should().BeFalse();
        }
        
        [DataRow("SS")]
        [DataRow("T")]
        [DataRow("")]
        [DataRow(null)]
        [DataTestMethod]
        public async Task NominatedPharmacyGet_ReturnsFalseWhenConfidentalityCodeIsPresent(string code)
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
                                   StringComparison.OrdinalIgnoreCase) && req.ControlActEvent.Query.RetrievalItems.Count == 4 &&
                                   req.ControlActEvent.Query.RetrievalItems[0].SemanticsText.Equals("person.nameUsual", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[1].SemanticsText.Equals("person.otherDemographics", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[2].SemanticsText.Equals("pharmacy", StringComparison.Ordinal) &&
                                   req.ControlActEvent.Query.RetrievalItems[3].SemanticsText.Equals("person.confidentiality", StringComparison.Ordinal))
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
                                                            }, 
                                                            Id = new Id
                                                            {
                                                                Extension = NhsNumberTrimmed
                                                            },
                                                            ConfidentialityCode = new ConfidentialityCode
                                                            {
                                                                Code = code
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.PharmacyOdsCode.Should().Be(null);
            result.HaveAllChecksPassed.Should().BeFalse();
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);

            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(httpStatusCode);
            result.PharmacyOdsCode.Should().Be(ExpectedPharmacyOdsCodeInResult);
            result.HaveAllChecksPassed.Should().BeFalse();
        }
        
        [DataTestMethod]
        public async Task
            NominatedPharmacyGet_ReturnsUnsuccessfulResponseWhenPharmacySearchFailsDueToClientFailure()
        {
            // Arrange
            _nominatedPharmacyClient
                .Setup(x => x.NominatedPharmacyGet(
                    It.IsAny<QUPAIN000008UK02>()
                )).Returns(Task.FromResult(GetUnsuccessfulNominatedPharmacyApiObjectResponse()))               
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetNominatedPharmacy(NhsNumber, _userSession.CitizenIdUserSession);
            
            // Assert
            _nominatedPharmacyClient.Verify();
            result.HttpStatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            result.HaveAllChecksPassed.Should().BeFalse();        
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
            var result = await _systemUnderTest.GetNominatedPharmacy(NhSNumber, _userSession.CitizenIdUserSession);

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