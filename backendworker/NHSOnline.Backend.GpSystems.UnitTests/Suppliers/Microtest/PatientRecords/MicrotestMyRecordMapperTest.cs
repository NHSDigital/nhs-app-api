using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using HtmlAgilityPack;
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
        public void MapPatientRecordGetResponse_ThrowsArgumentNullExceptionWhenObjectIsNull()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientRecordGetResponse");
        }

        /**
         * Map Allergy Tests
         */
        [TestMethod]
        public void MapPatientRecordGetResponse_MapAllergySuccessfully()
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
        public void MapPatientRecordGetResponse_ShouldNotMapAllergiesWhenSeverityIsNullOrEmpty(string severity)
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
        public void MapPatientRecordGetResponse_ShouldPutAllergiesWithValidDateBeforeAllergiesWithoutDate(string date)
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
        public void MapPatientRecordGetResponse_SummaryRecordAccessShouldBeFalseWhenNoAllergiesReturned()
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
        public void MapPatientRecordGetResponse_SummaryRecordAccessShouldBeFalseWhenAllAllergiesAreFilteredOut()
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
        public void MapPatientRecordGetResponse_ShouldOrderAllergiesInDecendingOrder()
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
        public void MapPatientRecordGetResponse_CanSuccessfullyMapSingleMedicationPerSection()
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
        public void MapPatientRecordGetResponse_CanSuccessfullyMapCurrentMedicationsOnly()
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
        public void MapPatientRecordGetResponse_CanSuccessfullyMapHistoricMedicationsOnly()
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
        public void MapPatientRecordGetResponse_CanSuccessfullyMapAcuteMedicationsOnly()
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
        public void MapPatientRecordGetResponse_CanSuccessfullyMultipleMedicationsForEachSection()
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
        public void MapPatientRecordGetResponse_ShouldNotMapMedicationItemsWithInvalidDates(String prescribedDate)
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
        public void MapPatientRecordGetResponse_ShouldSetSummaryAccessToFalseWhenNoMedicationsReturned()
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

        /*
         Map Immunisation Tests
         */
        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapSingleImmunisationItem()
        {
            //Arrange
            var item = new PatientRecordGetResponse
            {
                ImmunisationData = new ImmunisationData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Immunisations = new List<Immunisation>
                    {
                        BuildMicrotestImmunisation("2019-03-27", "Flu vaccination", "2022-03-27", "ok")
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.Immunisations.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();

            var expectedResult = new MyRecordResponse
            {
                Immunisations = new Immunisations
                {
                    Data = new List<ImmunisationItem>
                    {
                        BuildImmunisationItem(item.ImmunisationData.Immunisations.ElementAt(0))
                    }
                }
            };

            result.Immunisations.Should().BeEquivalentTo(expectedResult.Immunisations);
        }

        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("invalidDate")]
        public void MapPatientRecordGetResponse_ShouldPutImmunisationsWithNoEffectiveDateAfterThoseWithEffectiveDate(string date)
        {
            //Arrange
            var imm1 = BuildMicrotestImmunisation(date, "Mumps", "2022-03-27", "ok");
            var imm2 = BuildMicrotestImmunisation("2000-01-01", "Flu", "2022-03-27", "ok");
        
            var item = new PatientRecordGetResponse
            {
                ImmunisationData = new ImmunisationData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Immunisations = new List<Immunisation>
                    {
                        imm1, imm2        
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.Immunisations.Data.Should().HaveCount(2);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.ElementAt(0).Should().BeEquivalentTo(BuildImmunisationItem(imm2));
        }
        
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("no next date available")]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapImmunisationsWithUnparsableNextDate(string date)
        {
            //Arrange
            var imm1 = BuildMicrotestImmunisation("2019-01-01", "Mumps", date, "ok");
        
            var item = new PatientRecordGetResponse
            {
                ImmunisationData = new ImmunisationData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Immunisations = new List<Immunisation>
                    {
                        imm1        
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.Immunisations.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.ElementAt(0).Should().BeEquivalentTo(BuildImmunisationItem(imm1));
        }
        
        
        
        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderImmunisationsByDateDescending()
        {
            //Arrange
            var imm1 = BuildMicrotestImmunisation("2000-01-03", "Mumps", "2022-03-27", "ok");
            var imm2 = BuildMicrotestImmunisation("2000-01-01", "Flu", "2022-03-27", "ok");
            var imm3 = BuildMicrotestImmunisation("2000-01-02", "Measles", "2022-03-27", "ok");

            var item = new PatientRecordGetResponse
            {
                ImmunisationData = new ImmunisationData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Immunisations = new List<Immunisation>
                    {
                        imm1, imm2, imm3        
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);
            
            //Assert
            result.Should().NotBeNull();
            result.Immunisations.Data.Should().HaveCount(3);
            result.HasDetailedRecordAccess.Should().BeTrue();
            
            var expectedResult = new MyRecordResponse
            {
                Immunisations = new Immunisations
                {
                    Data = new List<ImmunisationItem>
                    {
                        BuildImmunisationItem(imm2),
                        BuildImmunisationItem(imm3),
                        BuildImmunisationItem(imm1),
                    }
                }
            };

            result.Immunisations.Should().BeEquivalentTo(expectedResult.Immunisations);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldSetDetailedRecordAccessToFalseWhenNoImmunisationsReturned()
        {
            var item = new PatientRecordGetResponse
            {
                ImmunisationData = new ImmunisationData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Immunisations = new List<Immunisation>()
                },
            };

            var result = _mapper.Map(item);

            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_WillMapProblemAndUseRawFinishDateValue_WhenFinishDateCannotBeParsed()
        {
            //Arrange
            var microtestProblem = BuildMicrotestProblem("2019-03-27","Ongoing","Angina");
            
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                ProblemData = new ProblemData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    Problems = new List<Problem>
                    {
                        microtestProblem
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Problems = new Problems
                {
                    Data = new List<ProblemItem>
                    {
                        BuildProblemItem(microtestProblem.StartDate, microtestProblem.FinishDate, microtestProblem.Rubric)
                    } 
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }     
 
        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapProblemsWhenFinishDateCanBeParsed()
        {
            //Arrange
            var microtestProblem = BuildMicrotestProblem("2019-03-27","2022-03-27","Angina");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                ProblemData = new ProblemData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    Problems = new List<Problem>
                    {
                        microtestProblem
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Problems = new Problems
                {
                    Data = new List<ProblemItem>
                    {
                        BuildProblemItem(microtestProblem.StartDate,"27 March 2022",microtestProblem.Rubric)
                    } 
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapProblemItemEvenWhenStartDateCannotBeParsed()
        {
            //Arrange
            var microtestProblem = BuildMicrotestProblem("no date","2022-03-27","Angina");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                ProblemData = new ProblemData
                {
                    Count = 1,
                    HasAccess = true,
                    HasErrored = false,
                    Problems = new List<Problem>
                    {
                        microtestProblem
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Problems = new Problems
                {
                    Data = new List<ProblemItem>
                    {
                        BuildProblemItem(microtestProblem.StartDate,"27 March 2022",microtestProblem.Rubric)
                    } 
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("No rubric recorded")]                            
        [DataRow(" No rubric recorded ")]         //spaces at start and end of string
        [DataRow("no RUBrIc RECOrded")]           //case insensitive check
        [DataRow("   NO rubric RECOrdED    ")]    //case insensitive check with leading and trailing spaces
        public void MapPatientRecordGetResponse_ShouldNotMapProblemItemWhenRubricValueIsNotValid(string rubric)
        {
            //Arrange
            var microtestProblem = BuildMicrotestProblem("2019-03-27","2022-03-27", rubric);

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                ProblemData = new ProblemData
                {
                    Count = 1,
                    HasAccess = true,
                    HasErrored = false,
                    Problems = new List<Problem>
                    {
                        microtestProblem
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Problems = new Problems
                {
                    Data = new List<ProblemItem>()
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().HaveCount(0);
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderProblemsByStartDateDescending()
        {
            //Arrange
            var prob1 = BuildMicrotestProblem("no date available","2022-03-27","Angina");          
            var prob2 = BuildMicrotestProblem("2019-03-27",       "2022-03-27","Gout");           
            var prob3 = BuildMicrotestProblem("2019-03-28",       "2022-03-27","Stroke");           
            var prob4 = BuildMicrotestProblem("no date available","2022-03-27","Migraine");     
            var prob5 = BuildMicrotestProblem("2019-03-26",       "Ongoing",   "Blind");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                ProblemData = new ProblemData
                {
                    Count = 5,
                    HasAccess = true,
                    HasErrored = false,
                    Problems = new List<Problem>
                    {
                        prob1, prob2, prob3, prob4, prob5
                    }
                },
            };

            //Expected Results
            var expectedResult = new MyRecordResponse
            {
                Problems = new Problems
                {
                    Data = new List<ProblemItem>
                    {
                        BuildProblemItem(prob3.StartDate, "27 March 2022", prob3.Rubric),
                        BuildProblemItem(prob2.StartDate, "27 March 2022", prob2.Rubric),
                        BuildProblemItem(prob5.StartDate, prob5.FinishDate, prob5.Rubric),  
                        BuildProblemItem(null, "27 March 2022", prob1.Rubric),   
                        BuildProblemItem(null, "27 March 2022", prob4.Rubric)   
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.Problems.Data.Should().HaveCount(5);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }
  
        private static AllergyItem BuildAllergyItem(Allergy allergy)
        {
            return new AllergyItem
            {
                Name = allergy.Description,
                Date = new MyRecordDate
                {
                    Value = DateTime.TryParse(allergy.StartDate, out var allergyDate)
                        ? allergyDate
                        : (DateTimeOffset?) null,
                    DatePart = "Unknown"
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
                Date = DateTime.Parse(item.PrescribedDate, CultureInfo.InvariantCulture),
                LineItems = medicationLineItems
            };
        }
        
        
        private static ImmunisationItem BuildImmunisationItem(Immunisation immunisation)
        {
            var item = new ImmunisationItem
            {
                Term = immunisation.Description,
                EffectiveDate = new MyRecordDate
                {
                    Value = DateTime.TryParse(immunisation.Date, out var effectiveDate)
                        ? effectiveDate
                        : (DateTimeOffset?) null,
                    DatePart = "Unknown",
                },
                Status = immunisation.Status
            };

            if (immunisation.NextDate != null)
            {
                item.NextDate = new MyRecordDateRawString();
                if (DateTime.TryParse(immunisation.NextDate, out var nextDate))
                {
                    item.NextDate.Value = nextDate;
                    item.NextDate.DatePart = "Unknown";
                }
                else
                {
                    item.NextDate.RawValue = immunisation.NextDate;
                }
            }   
                
            return item;  
        }
        
       private static ProblemItem BuildProblemItem(string startDate, string finishDate, string rubric)
        {
            return new ProblemItem
            {
                EffectiveDate = GetMyRecordDate(startDate),
                LineItems = new List<ProblemLineItem>
                {
                    new ProblemLineItem { Text = "Finish Date: " + finishDate },
                    new ProblemLineItem { Text = rubric }
                }
            };
        } 

        private static MyRecordDate GetMyRecordDate(string date)
        {
            if (DateTime.TryParse(date, out var validDate))
            {
                return new MyRecordDate
                {
                    Value = validDate,
                    DatePart = "Unknown"
                };
            } 
            return null;
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
        
        
        private static Immunisation BuildMicrotestImmunisation(string date, string desc, string nextDate, string status)
        {
            return new Immunisation
            {
                Date = date,
                Description = desc,
                NextDate = nextDate,
                Status = status
            };
        }
        
       
        private static Problem BuildMicrotestProblem(string startDate, string finishDate, string rubric)
        {
            return new Problem
            {
                StartDate = startDate,
                FinishDate = finishDate,
                Rubric = rubric,
            };
        }
    }

}