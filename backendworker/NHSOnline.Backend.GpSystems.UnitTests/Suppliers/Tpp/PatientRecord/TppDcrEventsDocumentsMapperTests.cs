using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDcrEventsDocumentsMapperTests
    {
        private TppDcrEventsDocumentsMapper _mapper;
        private ILogger<TppDcrEventsDocumentsMapper> _logger;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<TppDcrEventsDocumentsMapper>>();
            _mapper = new TppDcrEventsDocumentsMapper(_logger);
        }

        [TestMethod]
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var requestPatientRecordReply = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    new Event
                    {
                        Date = "2018-07-03T11:16:02.0Z",
                        DoneBy = "Mr General NhsApp",
                        Items = new List<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem
                            {
                                Details = "test: test.jpg",
                                Type = "Letter",
                                BinaryDataId = "123454"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "test: test1.jpg - with comments",
                                Type = "Attachment",
                                BinaryDataId = "123454"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "(R) Benzoin tincture - 500 ml - use as directed",
                                Type = "Medication"
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    },

                }
            };
            var expectedDocument = new PatientDocuments
            {
                Data = new List<DocumentItem>
                {
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse(requestPatientRecordReply.Events[0].Date,
                                CultureInfo.InvariantCulture)
                        },
                        DocumentIdentifier = requestPatientRecordReply.Events[0].Items[0].BinaryDataId,
                        IsAvailable = true,
                        Type = requestPatientRecordReply.Events[0].Items[0].Type,
                        IsValidFile = true,
                        Extension = "jpg",
                        NeedMoreInformation = true
                    },
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse(requestPatientRecordReply.Events[0].Date,
                                CultureInfo.InvariantCulture)
                        },
                        DocumentIdentifier = requestPatientRecordReply.Events[0].Items[1].BinaryDataId,
                        IsAvailable = true,
                        Type = "Document",
                        IsValidFile = true,
                        Comments = "with comments",
                        Extension = "jpg",
                        NeedMoreInformation = true
                    }
                },
                HasAccess = true,
                HasErrored = false,
            };

            // Act
            var tppDcrEvents = _mapper.Map(requestPatientRecordReply);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedDocument);

        }

        [TestMethod]
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithSupportedFileTypes_ReturnsMappedDocumentWithIsValidFileTrue()
        {
            // Arrange
            var requestPatientRecordReply = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    new Event
                    {
                        Date = "2018-07-03T11:16:02.0Z",
                        DoneBy = "Mr General NhsApp",
                        Items = new List<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem
                            {
                                Details = "test: test1.jpg - with comments",
                                Type = "Attachment",
                                BinaryDataId = "123454"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "test: test.rtf",
                                Type = "Attachment",
                                BinaryDataId = "123454"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "(R) Benzoin tincture - 500 ml - use as directed",
                                Type = "Medication"
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    },

                }
            };
            var expectedDate = DateTimeOffset.Parse("2018-07-03T11:16:02.0Z", CultureInfo.InvariantCulture);
            var expectedDocument = new PatientDocuments
            {
                Data = new List<DocumentItem>
                {
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = expectedDate,
                        },
                        DocumentIdentifier = "123454",
                        IsAvailable = true,
                        Type = "Document",
                        IsValidFile = true,
                        Comments = "with comments",
                        Extension = "jpg",
                        NeedMoreInformation = true,
                    },
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = expectedDate
                        },
                        DocumentIdentifier = "123454",
                        IsAvailable = true,
                        Type = "Document",
                        IsValidFile = true,
                        Extension = "rtf",
                        NeedMoreInformation = true
                    }
                },
                HasAccess = true,
                HasErrored = false,
            };

            // Act
            var tppDcrEvents = _mapper.Map(requestPatientRecordReply);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedDocument);

        }

        [TestMethod]
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithUnsupportedFileType_ReturnsMappedDocumentWithIsValidFileFalse()
        {
            // Arrange
            var requestPatientRecordReply = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    new Event
                    {
                        Date = "2018-07-03T11:16:02.0Z",
                        DoneBy = "Mr General NhsApp",
                        Items = new List<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem
                            {
                                Details = "test: test1.invalid - with comments",
                                Type = "Attachment",
                                BinaryDataId = "123454"
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    }

                }
            };
            var expectedDate = DateTimeOffset.Parse("2018-07-03T11:16:02.0Z", CultureInfo.InvariantCulture);
            var expectedDocument = new PatientDocuments
            {
                Data = new List<DocumentItem>
                {
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = expectedDate
                        },
                        DocumentIdentifier = "123454",
                        IsAvailable = true,
                        Type = "Document",
                        IsValidFile = false,
                        Comments = "with comments",
                        Extension = "invalid",
                        NeedMoreInformation = true
                    }
                },
                HasAccess = true,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(requestPatientRecordReply);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedDocument);

        }

        [DataTestMethod]
        [DataRow("test: test", "rtf")]
        [DataRow("test: test.jpg", "jpg")]
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithLetterFileType_ReturnsMappedDocumentWithTypeTxtIfNoTypeFoundAndIsValidTrue(string details, string expectedType)
        {
            // Arrange
            var requestPatientRecordReply = new RequestPatientRecordReply
            {
                Events = new List<Event>
                {
                    new Event
                    {
                        Date = "2018-07-03T11:16:02.0Z",
                        DoneBy = "Mr General NhsApp",
                        Items = new List<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem
                            {
                                Details = details,
                                Type = "Letter",
                                BinaryDataId = "123454"
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    }

                }
            };
            var expectedDate = DateTimeOffset.Parse("2018-07-03T11:16:02.0Z", CultureInfo.InvariantCulture);
            var expectedDocument = new PatientDocuments
            {
                Data = new List<DocumentItem>
                {
                    new DocumentItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = expectedDate
                        },
                        DocumentIdentifier = "123454",
                        IsAvailable = true,
                        Type = "Letter",
                        IsValidFile = true,
                        Extension = expectedType,
                        NeedMoreInformation = true
                    }
                },
                HasAccess = true,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(requestPatientRecordReply);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedDocument);

        }
    }
}