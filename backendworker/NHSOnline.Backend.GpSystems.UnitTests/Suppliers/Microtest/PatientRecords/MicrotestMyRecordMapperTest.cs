using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.PatientRecords
{
    [TestClass]
    public class MicrotestMyRecordMapperTest
    {
        private IMicrotestMyRecordMapper _mapper;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization()); 
            _mapper =  _fixture.Create<MicrotestMyRecordMapper>();
        }

        [TestMethod]
        public void MapNullObjectThrowsArgumentNullException()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientRecordGetResponse");
        }

        /**
         * Map Allergy Tests
         */
        [TestMethod]
        public void TestMapAllergiesWhenReturnsSuccess()
        {
            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>
                    {
                        BuildMicrotestAllergy(0, "low", "Nut Allergy", "2019-03-27")
                    }
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.Allergies.Data.Should().HaveCount(1);
            result.HasSummaryRecordAccess.Should().BeTrue();

            var expectedResult = new MyRecordResponse
            {
                Allergies = new Allergies
                {
                    Data = new List<AllergyItem>
                    {
                        BuildAllergyItem(item.AllergyData.Allergies.ElementAt(0))
                    }
                }
            };

            result.Allergies.Should().BeEquivalentTo(expectedResult.Allergies);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void TestAllergyWithNullOrEmptySeverityNotMapped(string severity)
        {
            var allergy1 = BuildMicrotestAllergy(0, severity, "Nut Allergy", "2019-03-27");
            var allergy2 = BuildMicrotestAllergy(1, "high", "Medication Allergy", "2019-04-27");
        
            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>
                    {
                        allergy1, allergy2        
                    }
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.Allergies.Data.Should().HaveCount(1);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("invalidDate")]
        public void TestAllergyWithNullOrInvalidDate(string date)
        {
            var allergy1 = BuildMicrotestAllergy(0, "low", "Nut Allergy", date);
            var allergy2 = BuildMicrotestAllergy(1, "high", "Medication Allergy", "2019-03-27");
        
            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>
                    {
                        allergy1, allergy2        
                    }
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.Allergies.Data.Should().HaveCount(2);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
        }

        [TestMethod]
        public void TestSummaryRecordAccessShouldBeFalseWhenNoAllergiesReturned()
        {
            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>()
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeFalse();
        }

        [TestMethod]
        public void TestSummaryRecordAccessShouldBeFalseWhenAllAllergiesAreFilteredOut()
        {
            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>
                    {
                        BuildMicrotestAllergy(0, null, "Nut Allergy", "2019-03-27"),
                        BuildMicrotestAllergy(1, "", "Medication Allergy", "2019-04-27"),          
                    }
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeFalse();
            result.Allergies.Data.Should().HaveCount(0);
        }

        [TestMethod]
        public void TestAllergiesOrderedInDescendingOrder()
        {
            var allergy1 = BuildMicrotestAllergy(0, "Medium", "Allergy A", "2019-03-27");
            var allergy2 = BuildMicrotestAllergy(1, "Severe", "Allergy B", "2019-04-27");
            var allergy3 = BuildMicrotestAllergy(2, "Low","Allergy C","2019-04-26");

            var item = new PatientRecordGetResponse
            {
                AllergyData = new AllergyData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Allergies = new List<Allergy>
                    {
                        allergy1, allergy2, allergy3,
                    }
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Allergies.Data.Should().HaveCount(3);      

            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
            result.Allergies.Data.ElementAt(1).Should().BeEquivalentTo(BuildAllergyItem(allergy3));
            result.Allergies.Data.ElementAt(2).Should().BeEquivalentTo(BuildAllergyItem(allergy1));
        }

        /**
         * Medication Tests
         */
        [TestMethod]
        public void MapMedicationSuccessSingleMedicationPerSection()
        {
            //Arrange
            var currentMedication = BuildCurrentMicrotestMedication(
                "Amoxycillan oral suspension sugar free",
                "500 mls",
                "10mls to be taken FOUR TIMES daily ",
                "2019-03-27",
                "Chest infection");

            var historicMedication = BuildHistoricMicrotestMedication(
                "Nitrofurantoin 25mg/5ml oral suspension sugar free",
                "300 mls",
                "ONE to be taken TWICE daily ",
                "2019-03-27",
                "Stable angina");

            var acuteMedication = BuildAcuteMicrotestMedication(
                "Cinchocaine 0.5% / Prednisolone 0.19% ointment",
                "30 grams",
                "No dosage recorded ",
                "2012-06-22",
                "No reason");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        currentMedication, historicMedication, acuteMedication
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        CurrentRepeatMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(currentMedication)
                        },
                        AcuteMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(acuteMedication)
                         },
                        DiscontinuedRepeatMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(historicMedication)
                        },
                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(1);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(1);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(1);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapMedicationSuccessForCurrentMedicationsOnly()
        {
            //Arrange
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 1,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        BuildCurrentMicrotestMedication("Amoxycillan oral suspension sugar free",
                            "500 mls",
                            "10mls to be taken FOUR TIMES daily ",
                            "2019-03-27",
                            "Chest infection"),
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        CurrentRepeatMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(microtestRecordResponse.MedicationData.Medications.ElementAt(0))
                        },
                        DiscontinuedRepeatMedications = new List<MedicationItem>(),
                        AcuteMedications = new List<MedicationItem>(),

                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(1);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapMedicationSuccessForHistoricMedicationsOnly()
        {
            //Arrange
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 1,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        BuildHistoricMicrotestMedication("Nitrofurantoin 25mg/5ml oral suspension sugar free",
                            "300 mls",
                            "ONE to be taken TWICE daily ",
                            "2019-03-27",
                            "Stable angina"),
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        DiscontinuedRepeatMedications = new List<MedicationItem>()
                        {
                            BuildMedicationItem(microtestRecordResponse.MedicationData.Medications.ElementAt(0))
                        },
                        AcuteMedications = new List<MedicationItem>(),
                        CurrentRepeatMedications = new List<MedicationItem>(),
                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(1);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapMedicationSuccessForAcuteMedicationsOnly()
        {
            //Arrange
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 1,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        BuildAcuteMicrotestMedication("Cinchocaine 0.5% / Prednisolone 0.19% ointment",
                            "30 grams",
                            "No dosage recorded ",
                            "2012-06-22",
                            "No reason"),
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        AcuteMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(microtestRecordResponse.MedicationData.Medications.ElementAt(0))
                        },
                        CurrentRepeatMedications = new List<MedicationItem>(),
                        DiscontinuedRepeatMedications = new List<MedicationItem>()
                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(1);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }


        [TestMethod]
        public void MapMedicationSuccessMultipleMedicationsForEachSection()
        {
            //Arrange
            var currentMedication = BuildCurrentMicrotestMedication(
                "Amoxycillan oral suspension sugar free",
                "500 mls",
                "10mls to be taken FOUR TIMES daily ",
                "2019-03-27",
                "Chest infection");

            var historicMedication = BuildHistoricMicrotestMedication(
                "Nitrofurantoin 25mg/5ml oral suspension sugar free",
                "300 mls",
                "ONE to be taken TWICE daily ",
                "2019-03-27",
                "Stable angina");

            var acuteMedication = BuildAcuteMicrotestMedication(
                "Cinchocaine 0.5% / Prednisolone 0.19% ointment",
                "30 grams",
                "No dosage recorded ",
                "2012-06-22",
                "No reason");

            //3 current medications, 2 historic medications, 4 acute medications
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 9,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        currentMedication, historicMedication, acuteMedication,
                        currentMedication, acuteMedication, acuteMedication,
                        currentMedication, historicMedication, acuteMedication
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        AcuteMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(acuteMedication),
                            BuildMedicationItem(acuteMedication),
                            BuildMedicationItem(acuteMedication),
                            BuildMedicationItem(acuteMedication)
                        }
                        ,
                        CurrentRepeatMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(currentMedication),
                            BuildMedicationItem(currentMedication),
                            BuildMedicationItem(currentMedication)
                        },
                        DiscontinuedRepeatMedications = new List<MedicationItem>
                        {
                            BuildMedicationItem(historicMedication),
                            BuildMedicationItem(historicMedication),
                        }
                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(4);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(3);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(2);
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }
        
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("no last prescribed date")]
        [DataRow("")]
        public void MapMedicationInvalidDatesNotMapped(String prescribedDate)
        {
            //Arrange
            var currentMedication = BuildCurrentMicrotestMedication(
                "Amoxycillan oral suspension sugar free",
                "500 mls",
                "10mls to be taken FOUR TIMES daily ",
                prescribedDate,
                "Chest infection");

            var historicMedication = BuildHistoricMicrotestMedication(
                "Nitrofurantoin 25mg/5ml oral suspension sugar free",
                "300 mls",
                "ONE to be taken TWICE daily ",
                prescribedDate,
                "Stable angina");

            var acuteMedication = BuildAcuteMicrotestMedication(
                "Cinchocaine 0.5% / Prednisolone 0.19% ointment",
                "30 grams",
                "No dosage recorded ",
                prescribedDate,
                "No reason");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>
                    {
                        currentMedication, historicMedication, acuteMedication
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Medications = new Medications
                {
                    Data = new MedicationsData
                    {
                        CurrentRepeatMedications = new List<MedicationItem>(),
                        AcuteMedications = new List<MedicationItem>(),
                        DiscontinuedRepeatMedications = new List<MedicationItem>(),
                    }
                },
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.HasSummaryRecordAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }
        
        

        [TestMethod]
        public void TestSummaryRecordAccessShouldBeFalseWhenNoMedicationsReturned()
        {
            //Input data
            var item = new PatientRecordGetResponse
            {
                MedicationData = new MedicationData()
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Medications = new List<Medication>()
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeFalse();
        }

        private static AllergyItem BuildAllergyItem(Allergy allergy)
        {
            return new AllergyItem
            {
                Name = allergy.Description,
                Date = new MyRecordDate
                {
                    Value = DateTime.TryParse(allergy.StartDate, out var eventDate)
                        ? eventDate
                        : (DateTimeOffset?) null,
                    DatePart = allergy.StartDate
                }
            };
        }

        private static MedicationItem BuildMedicationItem(Medication item)
        {
            var medicationLineItems = new List<MedicationLineItem>();
            medicationLineItems.AddRange(new List<MedicationLineItem>
            {
                new MedicationLineItem { Text = item.Name },
                new MedicationLineItem { Text = item.Dosage },
                new MedicationLineItem { Text = item.Quantity },
                new MedicationLineItem { Text = "Reason: " + item.Reason }
            });

            return new MedicationItem
            {
                Date = DateTime.Parse(item.PrescribedDate, new CultureInfo("en-GB")),
                LineItems = medicationLineItems
            };
        }

        private static Allergy BuildMicrotestAllergy(int id, string severity, string desc, string startDate)
        {
            return new Allergy
            {
                Id = id,
                Severity = severity,
                Description = desc,
                StartDate = startDate,
                Type = "Allergy"
            };
        }

        private static Medication BuildAcuteMicrotestMedication(string name, string quantity, string dosage, string date, string reason)
        {
            return BuildMicrotestMedication(name, quantity, dosage, MedicationStatus.Acute, MedicationType.Historic, date, reason);
        }

        private static Medication BuildCurrentMicrotestMedication(string name, string quantity, string dosage, string date, string reason)
        {
            return BuildMicrotestMedication(name, quantity, dosage, MedicationStatus.Repeat, MedicationType.Current, date, reason);
        }

        private static Medication BuildHistoricMicrotestMedication(string name, string quantity, string dosage, string date, string reason)
        {
            return BuildMicrotestMedication(name, quantity, dosage, MedicationStatus.Repeat, MedicationType.Historic, date, reason);
        }

        private static Medication BuildMicrotestMedication(string name, string quantity, string dosage, string status, string type, string date, string reason)
        {
            return new Medication
            {
                Id = 0,
                Name = name,
                Quantity = quantity,
                Dosage = dosage,
                Status = status,
                Type = type,
                PrescribedDate = date,
                Reason = reason
            };
        }
    }

}