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
    public class EmisProblemMapperTests
    {
        private IFixture _fixture;
        private IEmisMyRecordMapper _mapper;
        private const string DATE_FORMAT = "d MMMM yyyy";

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisMyRecordMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        } 

        [TestMethod]
        public void MapProblemRequestsGetResponseToProblemListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new MedicationRootObject();

            // Act
            var result = _mapper.Map(new Allergies(), new Medications(), new Immunisations(), new TestResults(), new EmisProblemMapper().Map(item), new Consultations());

            // Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().BeEmpty();
        }

        [TestMethod]
        public void MapProblemRequestsGetResponseToProblemListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Problems = new List<Problem>
                    {
                        new Problem
                        {
                            Status = _fixture.Create<string>(),
                            Significance = _fixture.Create<string>(),
                            ProblemEndDate = _fixture.Create<DateTime>(),
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
                        new Problem
                        {
                            Status = _fixture.Create<string>(),
                            Significance = _fixture.Create<string>(),
                            ProblemEndDate = _fixture.Create<DateTime>(),
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
            var result = new EmisProblemMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Problems.Count);

            var problem1 = item.MedicalRecord.Problems.ElementAt(0);
            var problem2 = item.MedicalRecord.Problems.ElementAt(1);
            
            var expectedResult = new Problems
            {
                Data = new List<ProblemItem>
                {
                    new ProblemItem
                    {
                        EffectiveDate = new Date { Value = problem1.Observation.EffectiveDate.Value, DatePart = problem1.Observation.EffectiveDate.DatePart },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem
                            {
                                Text = problem1.Observation.Term
                            },
                            new ProblemLineItem
                            {
                                Text = "Significance: " + problem1.Significance
                            },
                            new ProblemLineItem
                            {
                                Text = "Status: " + problem1.Status
                            },
                            new ProblemLineItem
                            {
                                Text = "Notes:",
                                LineItems = new List<string>
                                {
                                    problem1.Observation.AssociatedText[0].Text,
                                    problem1.Observation.AssociatedText[1].Text,
                                    problem1.Observation.AssociatedText[2].Text,
                                }
                            },
                            new ProblemLineItem
                            {
                                Text = "Ended: " + problem1.ProblemEndDate.Value.ToString(DATE_FORMAT)
                            },
                        }
                    },
                    new ProblemItem
                    {
                        EffectiveDate = new Date { Value = problem2.Observation.EffectiveDate.Value, DatePart = problem2.Observation.EffectiveDate.DatePart },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem
                            {
                                Text = problem2.Observation.Term
                            },
                            new ProblemLineItem
                            {
                                Text = "Significance: " + problem2.Significance
                            },
                            new ProblemLineItem
                            {
                                Text = "Status: " + problem2.Status
                            },
                            new ProblemLineItem
                            {
                                Text = "Notes:",
                                LineItems = new List<string>
                                {
                                    problem2.Observation.AssociatedText[0].Text,
                                    problem2.Observation.AssociatedText[1].Text,
                                }
                            },
                            new ProblemLineItem
                            {
                                Text = "Ended: " + problem2.ProblemEndDate.Value.ToString(DATE_FORMAT)
                            },
                        }
                    },
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }       
    }
}