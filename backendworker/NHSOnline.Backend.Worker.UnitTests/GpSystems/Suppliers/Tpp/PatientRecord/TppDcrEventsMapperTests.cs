using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDcrEventsMapperTests
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
            var item = new RequestPatientRecordReply();

            // Act
            var tppDcrEvents = new TppDcrEventsMapper().Map(item);
            var result = _mapper.Map(new Allergies(), new Medications(), tppDcrEvents, new TestResults());

            // Assert
            result.Should().NotBeNull();
            result.TppDcrEvents.Data.Should().BeEmpty();           
        }

        [TestMethod]
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithValues_ReturnsResultValues()
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
                            new RequestPatientRecordItem { 
                                Details = "Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]", 
                                Type = "Medication template"                             
                            },
                            new RequestPatientRecordItem { 
                                Details = "Benzoin tincture - 500 ml - use as directed", 
                                Type = "Medication template"                             
                            },
                            new RequestPatientRecordItem { 
                                Details = "(R) Benzoin tincture - 500 ml - use as directed", 
                                Type = "Medication"                             
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    },
                    new Event
                    {
                        Date = "2016-09-12T12:34:03.0Z", 
                        DoneBy = "Mr General NhsApp 2", 
                        Items = new List<RequestPatientRecordItem>
                        {
                            new RequestPatientRecordItem { 
                                Details = "Alimemazine 20mg tablets - 1 pack of 14 tablet(s)", 
                                Type = "Medication template"                             
                            },
                            new RequestPatientRecordItem { 
                                Details = "(R) Benzoin tincture - 250 ml - use as directed", 
                                Type = "Medication"                             
                            }
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    }
                }
                
            };


            var tppDcrEvents = new List<TppDcrEvent>
            {
                new TppDcrEvent
                {
                    Date = DateTimeOffset.Parse(requestPatientRecordReply.Events[0].Date),
                    LocationAndDoneBy = $"{requestPatientRecordReply.Events[0].Location} - {requestPatientRecordReply.Events[0].DoneBy}",
                    EventItems = new List<string> { 
                        $"{requestPatientRecordReply.Events[0].Items[0].Type} - { requestPatientRecordReply.Events[0].Items[0].Details}",
                        $"{requestPatientRecordReply.Events[0].Items[1].Type} - { requestPatientRecordReply.Events[0].Items[1].Details}",
                        $"{requestPatientRecordReply.Events[0].Items[2].Type} - { requestPatientRecordReply.Events[0].Items[2].Details}"     
                    }
                },
                new TppDcrEvent
                {
                    Date = DateTimeOffset.Parse(requestPatientRecordReply.Events[1].Date),
                    LocationAndDoneBy = $"{requestPatientRecordReply.Events[1].Location} - {requestPatientRecordReply.Events[1].DoneBy}",
                    EventItems = new List<string> { 
                        $"{requestPatientRecordReply.Events[1].Items[0].Type} - { requestPatientRecordReply.Events[1].Items[0].Details}",
                        $"{requestPatientRecordReply.Events[1].Items[1].Type} - { requestPatientRecordReply.Events[1].Items[1].Details}",    
                    }
                }                
            };
            // Act
            var result = new TppDcrEventsMapper().Map(requestPatientRecordReply);
            
            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(requestPatientRecordReply.Events.Count());
            result.Data.Should().BeEquivalentTo(tppDcrEvents);
        }

        [TestMethod]
        public void MapPatientRecordReplyToDCREvent_WithEventWhichContainsNewLineCharacter_ReturnsDCREventWithNewLineCharacterRemoved()
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
                            new RequestPatientRecordItem { 
                                Details = "\nBenzoin tincture - 500 ml\nuse as\t directed\n", 
                                Type = "Medication template"                             
                            },
                        },
                        Location = "Kainos GP Demo Unit (General Practice)"
                    },
                }              
            };

            var result = new TppDcrEventsMapper().Map(requestPatientRecordReply);

            result.Should().NotBeNull();
            result.Data[0].EventItems[0].Should().Be("Medication template - Benzoin tincture - 500 ml; use as directed");
        }
    }
}