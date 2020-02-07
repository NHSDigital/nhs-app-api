using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDcrEventsDocumentsMapperTests
    {
        private ITppMyRecordMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppMyRecordMapper();
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
                                Details = "test",
                                Type = "Letter",
                                BinaryDataId = "123454"
                            },
                            new RequestPatientRecordItem
                            {
                                Details = "test 2",
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
                        Type = requestPatientRecordReply.Events[0].Items[0].Type
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
                        Type = "Document"
                    }
                },
                HasAccess = true,
                HasErrored = false,
            };

            // Act
            var tppDcrEvents = new TppDcrEventsDocumentsMapper().Map(requestPatientRecordReply);
            //var result = _mapper.Map(new Allergies(), new Medications(), tppDcrEvents, new TestResults(), new PatientDocuments());

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedDocument);

        }
    }
}