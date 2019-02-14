using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisMedicationMapperTests
    {
        private IEmisMedicationMapper _mapper;
        private const string DateFormat = "d MMMM yyyy";

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisMedicationMapper();
        } 

        [TestMethod]
        public void MapMedicationRequestsGetResponseToMedicationListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("medicationRootObject");
        }   

        [TestMethod]
        public void MapMedicationRequestsGetResponseToMedicationListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new MedicationRootObject();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.AcuteMedications.Should().BeEmpty();
            result.Data.CurrentRepeatMedications.Should().BeEmpty();
            result.Data.DiscontinuedRepeatMedications.Should().BeEmpty();
        }

        [TestMethod]
        public void MapMedicationRequestsGetResponseToMedicationListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var now = DateTimeOffset.Now;
            var oneYearAgo = now.AddYears(-1);
            var twoYearsAgo = now.AddYears(-2);
            
            var item = new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord 
                {
                    Medication = GetSampleMedicationResponse(now)
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.AcuteMedications.Should().HaveCount(2);
            result.Data.CurrentRepeatMedications.Should().HaveCount(2);
            result.Data.DiscontinuedRepeatMedications.Should().HaveCount(2);

            var expectedResult = new Medications
            {
                Data = new MedicationsData
                {
                    AcuteMedications = new List<MedicationItem>
                    {
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "First Acute Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken twice a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "14 capsules",
                                },
                            }
                        },
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "Second Acute Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken four times a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "28 capsules",
                                },
                            }
                        },
                    },
                    CurrentRepeatMedications = new List<MedicationItem>
                    {
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "First Repeat Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "MegaMix, consisting of:",
                                    LineItems = new List<string>
                                    {
                                        "Ibuprofen oral suspension - 100ml",
                                        "Paracetamol oral suspension - 50ml",
                                    }
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken twice a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "14 capsules",
                                },
                            }
                        },
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "Second Repeat Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken twice a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "14 capsules",
                                },
                            }
                        },
                    },
                    DiscontinuedRepeatMedications = new List<MedicationItem>
                    {
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "First Repeat Cancelled Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken twice a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "14 capsules",
                                },
                                new MedicationLineItem
                                {
                                    Text = "Ended: " + oneYearAgo.ToString(DateFormat, CultureInfo.InvariantCulture),
                                },
                            }
                        },
                        new MedicationItem
                        {
                            Date = oneYearAgo,
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem
                                {
                                    Text = "Second Repeat Cancelled Drug",
                                },
                                new MedicationLineItem
                                {
                                    Text = "One taken twice a day",
                                },
                                new MedicationLineItem
                                {
                                    Text = "14 capsules",
                                },
                                new MedicationLineItem
                                {
                                    Text = "Ended: " + twoYearsAgo.ToString(DateFormat, CultureInfo.InvariantCulture),
                                },
                            }
                        },
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        private List<Medication> GetSampleMedicationResponse(DateTimeOffset now)
        {
            // List consists of
            // 3 acute medications, 2 in the last year
            // 2 active repeat medications
            // 1 inactive repeat medication
            var result = new List<Medication>
            {
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "Second Acute Drug",
                    IsMixture = false,
                    Dosage = "One taken four times a day",
                    QuantityRepresentation = "28 capsules",
                    PrescriptionType = "Acute",
                    LastIssueDate = now.AddMonths(-1)
                },
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "First Acute Drug",
                    IsMixture = false,
                    Dosage = "One taken twice a day",
                    QuantityRepresentation = "14 capsules",
                    PrescriptionType = "Acute",
                    LastIssueDate = now
                },
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "Removed Acute Drug",
                    IsMixture = false,
                    PrescriptionType = "Acute",
                    LastIssueDate = now.AddYears(-1)
                },
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "First Repeat Drug",
                    IsMixture = true,
                    Mixture = new Mixture
                    {
                        MixtureName = "MegaMix",
                        Constituents = new List<Constituent>
                        {
                            new Constituent
                            {
                                ConstituentName = "Ibuprofen oral suspension",
                                Strength = "100ml",
                            },
                            new Constituent
                            {
                                ConstituentName = "Paracetamol oral suspension",
                                Strength = "50ml",
                            }
                        }
                    },
                    Dosage = "One taken twice a day",
                    QuantityRepresentation = "14 capsules",
                    PrescriptionType = "Repeat",
                    DrugStatus = "Active",
                    LastIssueDate = now
                },
                new Medication
                {
                    Term = "Second Repeat Drug",
                    IsMixture = false,
                    Dosage = "One taken twice a day",
                    QuantityRepresentation = "14 capsules",
                    PrescriptionType = "Repeat",
                    DrugStatus = "Active",
                    FirstIssueDate = now.AddYears(-1),
                    LastIssueDate = now.AddMonths(-1)
                },
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "Second Repeat Cancelled Drug",
                    IsMixture = false,
                    Dosage = "One taken twice a day",
                    QuantityRepresentation = "14 capsules",
                    PrescriptionType = "Repeat",
                    DrugStatus = "Cancelled",
                    LastIssueDate = now.AddYears(-2)
                },
                new Medication
                {
                    FirstIssueDate = now.AddYears(-1),
                    Term = "First Repeat Cancelled Drug",
                    IsMixture = false,
                    Dosage = "One taken twice a day",
                    QuantityRepresentation = "14 capsules",
                    PrescriptionType = "Repeat",
                    DrugStatus = "Cancelled",
                    LastIssueDate = now.AddYears(-1)
                },
            }; 
            
            return result;
        }
    }
}
