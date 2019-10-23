using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisDocumentsMapperTests
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
        public void MapDocumentRequestsGetResponseToDocumentListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new MedicationRootObject();

            // Act
            var result = _mapper.Map(new Allergies(), new Medications(), new Immunisations(), new TestResults(), new Problems(), new Consultations(), new EmisDocumentsMapper().Map(item));

            // Assert
            result.Should().NotBeNull();
            result.Documents.Data.Should().BeEmpty();
        }

        [TestMethod]
        public void MapDocumentsRequestsGetResponseToDocumentListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Documents = new List<Document>
                    {
                        new Document
                        {
                            DocumentGuid = _fixture.Create<string>(),
                            Size = _fixture.Create<int>(),
                            PageCount = _fixture.Create<int>(),
                            Extension = _fixture.Create<string>(),
                            Observation = new Observation
                            {
                                EffectiveDate = new EffectiveDate
                                {
                                    DatePart = "Unknown",
                                    Value = _fixture.Create<DateTime>()
                                },
                                Term = _fixture.Create<string>(),
                                AssociatedText = new List<AssociatedText>
                                {
                                    new AssociatedText
                                    {
                                        Text = _fixture.Create<string>()
                                    },
                                    new AssociatedText
                                    {
                                        Text = _fixture.Create<string>()
                                    },
                                    new AssociatedText
                                    {
                                        Text = _fixture.Create<string>()
                                    }
                                }
                            }
                        },
                        new Document
                        {
                            DocumentGuid = _fixture.Create<string>(),
                            Size = _fixture.Create<int>(),
                            PageCount = _fixture.Create<int>(),
                            Extension = _fixture.Create<string>(),
                            Observation = new Observation
                            {
                                EffectiveDate = new EffectiveDate
                                {
                                    DatePart = "Unknown",
                                    Value = _fixture.Create<DateTime>()
                                },
                                Term = _fixture.Create<string>(),
                                AssociatedText  = new List<AssociatedText>
                                {
                                    new AssociatedText
                                    {
                                        Text = _fixture.Create<string>()
                                    },
                                    new AssociatedText
                                    {
                                        Text = _fixture.Create<string>()
                                    }
                                }
                            }
                        },
                    },
                }
            };
            
            // Act
            var result = new EmisDocumentsMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Documents.Count);

            var document1 = item.MedicalRecord.Documents.ElementAt(0);
            var document2 = item.MedicalRecord.Documents.ElementAt(1);
            
            var expectedResult = new PatientDocuments
            {
                Data = new List<DocumentItem>
                {
                    new DocumentItem
                    {
                        DocumentGuid = document1.DocumentGuid,
                        Term = document1.Observation.Term,
                        IsAvailable = document1.Available,
                        Extension = document1.Extension,
                        Size = document1.Size,
                        EffectiveDate = new MyRecordDate { Value = document1.Observation.EffectiveDate.Value, DatePart = document1.Observation.EffectiveDate.DatePart },
                        Name = document1.Observation.AssociatedText[0].Text
                    },
                    new DocumentItem
                    {
                        DocumentGuid = document2.DocumentGuid,
                        Term = document2.Observation.Term,
                        IsAvailable = document2.Available,
                        Extension = document2.Extension,
                        Size = document2.Size,
                        EffectiveDate = new MyRecordDate { Value = document2.Observation.EffectiveDate.Value, DatePart = document2.Observation.EffectiveDate.DatePart },
                        Name = document2.Observation.AssociatedText[0].Text
                    },
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }       
    }
}
