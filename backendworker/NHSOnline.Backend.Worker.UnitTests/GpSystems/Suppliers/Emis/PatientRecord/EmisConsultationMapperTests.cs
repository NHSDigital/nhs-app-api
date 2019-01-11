using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisConsultationMapperTests
    {
        private IFixture _fixture;
        private IEmisMyRecordMapper _mapper;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisMyRecordMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        } 

        [TestMethod]
        public void MapConsultationRequestGetResponseToConsultationListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new MedicationRootObject();

            // Act
            var result = _mapper.Map(new Allergies(), new Medications(), new Immunisations(), new TestResults(), new EmisProblemMapper().Map(item), new Consultations());

            // Assert
            result.Should().NotBeNull();
            result.Consultations.Data.Should().BeEmpty();
        }

        [TestMethod]
        public void MapConsultationRequestGetResponseToConsultationListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => new EmisConsulationMapper().Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("consultationsGetResponse");
        }

        [TestMethod]
        public void MapConsultationRequestsGetResponseToConsultationListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Consultations = new List<Consultation>
                    {
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Jean (Dr)",
                            Location = "THE SURGERY - MOSS", 
                            Sections = new List<Section>
                            {
                                new Section { Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                    new Observation { 
                                        Term = "test observation term 1", 
                                        AssociatedText = new List<AssociatedText> { new AssociatedText { Text = "Tired generally. Needs to have bloods etc" }},
                                        ObservationType = "Unknown"
                                        }  
                                    },
                                },
                            },
                        },
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Robert (Dr)",
                            Location = "THE SURGERY - LAMBERT",
                            Sections = new List<Section>
                            {
                                new Section { Header = "History 2", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "test observation term 2", 
                                            AssociatedText = new List<AssociatedText>
                                            {
                                                new AssociatedText { Text = "steroids and see friday week" },
                                                new AssociatedText { Text = "steroids and see friday week 2" }
                                            },
                                            ObservationType = "Unknown"
                                        }  
                                    },
                                },
                            },
                        },
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "James (Dr)",
                            Location = "THE SURGERY - EDINBURGH",
                            Sections = new List<Section>
                            {
                                new Section { Header = "History 3", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "test observation term 3", 
                                            AssociatedText = null,
                                            ObservationType = "Observation",
                                        }  
                                    },
                                },
                            },
                        },
                    },
                }
            };
                
            // Act
            var result = new EmisConsulationMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Consultations.Count);

            var consultation1 = item.MedicalRecord.Consultations.ElementAt(0);
            var consultation2 = item.MedicalRecord.Consultations.ElementAt(1);
            var consultation3 = item.MedicalRecord.Consultations.ElementAt(2);
            
            var expectedResult = new Consultations
            {
                Data = new List<ConsultationItem>
                {
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation1.EffectiveDate.Value, DatePart = consultation1.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation1.Location} - {consultation1.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem { Header = consultation1.Sections[0].Header, 
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { new ObservationItemWithTerm
                            {
                                Term = consultation1.Sections[0].Observations[0].Term,
                                AssociatedTexts = consultation1.Sections[0].Observations[0].AssociatedText.Select(x=>x.Text).ToList()
                            }}}    
                        },
                    },
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation2.EffectiveDate.Value, DatePart = consultation2.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation2.Location} - {consultation2.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem { Header = consultation2.Sections[0].Header, 
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { new ObservationItemWithTerm
                                {
                                    Term = consultation2.Sections[0].Observations[0].Term,
                                    AssociatedTexts = consultation2.Sections[0].Observations[0].AssociatedText.Select(x=>x.Text).ToList()
                                }}}    
                        },
                    },   
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation3.EffectiveDate.Value, DatePart = consultation3.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation3.Location} - {consultation3.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem { 
                                Header = consultation3.Sections[0].Header, 
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { new ObservationItemWithTerm
                                {
                                    Term = consultation3.Sections[0].Observations[0].Term,
                                    AssociatedTexts = new List<string>()
                                }}}
                        },
                    },   
                }
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void MapConsultationRequestsGetResponseToConsultationListResponse_WithObservationTypesOtherThanConsultationAndUnknown_ReturnsOnlyObservationsWithTypeObservationOrUnknown()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Consultations = new List<Consultation>
                    {
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Jean (Dr)",
                            Location = "THE SURGERY - MOSS", 
                            Sections = new List<Section>
                            {
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                    new Observation { 
                                            Term = "test observation term 1", 
                                            AssociatedText = new List<AssociatedText> { new AssociatedText { Text = "Tired generally. Needs to have bloods etc" }},
                                            ObservationType = "Other"
                                        },
                                    new Observation { 
                                        Term = "test observation term 1b", 
                                        AssociatedText = new List<AssociatedText> { new AssociatedText { Text = "Unwell" }},
                                        ObservationType = "Unknown"
                                    },
                                    new Observation { 
                                        Term = null, 
                                        AssociatedText = null,
                                        ObservationType = "Unknown"
                                    },
                                  },                                
                                },
                                new Section
                                {
                                    Header = "History 2",
                                    Observations = new List<Observation>
                                    {
                                        new Observation { 
                                            Term = null, 
                                            AssociatedText = new List<AssociatedText> { new AssociatedText { Text = "Tired" }},
                                            ObservationType = "Other"
                                        }, 
                                    }
                                }
                            },
                        },
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Robert (Dr)",
                            Location = "THE SURGERY - LAMBERT",
                            Sections = new List<Section>
                            {
                                new Section { 
                                    Header = "History 2", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "test observation term 2", 
                                            AssociatedText = new List<AssociatedText>
                                            {
                                                new AssociatedText { Text = "steroids and see friday week" },
                                                new AssociatedText { Text = "steroids and see friday week 2" }
                                            },
                                            ObservationType = "Unknown"
                                        }  
                                    },
                                },
                            },
                        },
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "James (Dr)",
                            Location = "THE SURGERY - EDINBURGH",
                            Sections = new List<Section>
                            {
                                new Section { 
                                    Header = "History 3", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "test observation term 3", 
                                            AssociatedText = null,
                                            ObservationType = "Observation",
                                        }  
                                    },
                                },
                            },
                        },
                    },
                }
            };
                
            // Act
            var result = new EmisConsulationMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Consultations.Count);

            var consultation1 = item.MedicalRecord.Consultations.ElementAt(0);
            var consultation2 = item.MedicalRecord.Consultations.ElementAt(1);
            var consultation3 = item.MedicalRecord.Consultations.ElementAt(2);
            
            var expectedResult = new Consultations
            {
                Data = new List<ConsultationItem>
                {
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation1.EffectiveDate.Value, DatePart = consultation1.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation1.Location} - {consultation1.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem
                            {
                                Header = consultation1.Sections[0].Header,
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { 
                                new ObservationItemWithTerm
                                    {
                                        Term = consultation1.Sections[0].Observations[1].Term,
                                        AssociatedTexts = consultation1.Sections[0].Observations[1].AssociatedText.Select(x=>x.Text).ToList()
                                    },
                                },
                            },
                        },
                    },
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation2.EffectiveDate.Value, DatePart = consultation2.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation2.Location} - {consultation2.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem { 
                                Header = consultation2.Sections[0].Header, 
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { new ObservationItemWithTerm
                                {
                                    Term = consultation2.Sections[0].Observations[0].Term,
                                    AssociatedTexts = consultation2.Sections[0].Observations[0].AssociatedText.Select(x=>x.Text).ToList()
                                }}}    
                        },
                    },   
                    new ConsultationItem
                    {
                        EffectiveDate = new MyRecordDate { Value = consultation3.EffectiveDate.Value, DatePart = consultation3.EffectiveDate.DatePart },
                        ConsultantLocation = $"{consultation3.Location} - {consultation3.ConsultantName}",
                        ConsultationHeaders = new List<ConsultationHeaderItem>
                        {
                            new ConsultationHeaderItem { Header = consultation3.Sections[0].Header, 
                                ObservationsWithTerm = new List<ObservationItemWithTerm> { new ObservationItemWithTerm
                                {
                                    Term = consultation3.Sections[0].Observations[0].Term,
                                    AssociatedTexts = new List<string>()
                                }}}    
                        },
                    },   
                }
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void MapConsultationRequestsGetResponseToConsultationListResponse_WithNewLineAndTabCharactersInAssociatedText_ReturnsAssociatedTextWithNewLineAndTabCharactersRemoved()
        {
             // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Consultations = new List<Consultation>
                    {
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Jean (Dr)",
                            Location = "THE SURGERY - MOSS",
                            Sections = new List<Section>
                            {
                                new Section { Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                    new Observation { 
                                        Term = "test observation term 1", 
                                        AssociatedText = new List<AssociatedText> { new AssociatedText { Text = "\nTired generally\nNeeds to have\t bloods etc\n\t" }},
                                        ObservationType = "Observation"
                                        }  
                                    },
                                },
                            },
                        },
                      
                    },
                }
            };
            
            // Act
            var result = new EmisConsulationMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.ToList()[0].ConsultationHeaders[0].ObservationsWithTerm[0].AssociatedTexts[0].Should().Be("Tired generally; Needs to have bloods etc");
        }
        
        [TestMethod]
        public void MapConsultationRequestsGetResponseToConsultationListResponse_WithSectionHeaderButNoTermOrAssociatedText_ReturnsConsultationsWithNoSection()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Consultations = new List<Consultation>
                    {
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Jean (Dr)",
                            Location = "THE SURGERY - MOSS",
                            Sections = new List<Section>
                            {
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "",
                                            AssociatedText = new List<AssociatedText>(),
                                            ObservationType = "Observation"
                                        }  
                                    },
                                    
                                },
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "",
                                            AssociatedText = null, 
                                            ObservationType = "Observation"
                                        }  
                                    },
                                    
                                },
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = null,
                                            AssociatedText = null,
                                            ObservationType = "Observation"
                                        }  
                                    },
                                    
                                },
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term =  null,
                                            AssociatedText = new List<AssociatedText>(),
                                            ObservationType = "Observation"
                                        }  
                                    },
                                    
                                },
                            },
                        },                     
                    },  
                },
            };
            
            // Act
            var result = new EmisConsulationMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.ToList()[0].ConsultationHeaders.Should().HaveCount(0);
        }

        [TestMethod]
        public void MapConsultationRequestsGetResponseToConsultationListResponse_WithNoConsultantLocationAndConsultantName_ReturnsConsultantName()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Consultations = new List<Consultation>
                    {
                        new Consultation
                        {
                            EffectiveDate = new EffectiveDate { DatePart = "D MMMMM YYYY", Value = _fixture.Create<DateTime>() },
                            ConsultantName = "Jean (Dr)",
                            Location = "",
                            Sections = new List<Section>
                            {
                                new Section { 
                                    Header = "History 1", 
                                    Observations  = new List<Observation> { 
                                        new Observation { 
                                            Term = "",
                                            AssociatedText = new List<AssociatedText>(),
                                            ObservationType = "Observation",
                                        }  
                                    }, 
                                },
                            },
                        },                     
                    },  
                },
            };
            
            // Act
            var result = new EmisConsulationMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.ToList()[0].ConsultantLocation.Should().Be(item.MedicalRecord.Consultations[0].ConsultantName);
        }
    }
}