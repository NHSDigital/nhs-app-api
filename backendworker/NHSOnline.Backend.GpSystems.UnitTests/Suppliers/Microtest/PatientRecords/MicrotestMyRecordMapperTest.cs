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
    [TestClass] public class MicrotestMyRecordMapperTest
    {
        private IMicrotestMyRecordMapper _mapper;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mapper = _fixture.Create<MicrotestMyRecordMapper>();
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ThrowsArgumentNullExceptionWhenObjectIsNull()
        {
            // Act
            Action act = () => _mapper.Map(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientRecordGetResponse");
        }
        
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
            result.Allergies.HasUndeterminedAccess.Should().BeFalse();
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
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Allergies.Data.Should().HaveCount(1);
            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
            result.Allergies.HasUndeterminedAccess.Should().BeFalse();
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
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Allergies.Data.Should().HaveCount(2);
            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
            result.Allergies.HasUndeterminedAccess.Should().BeFalse();
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
            result.Allergies.HasUndeterminedAccess.Should().BeTrue();
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
            result.Allergies.HasUndeterminedAccess.Should().BeFalse();
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderAllergiesInDescendingOrder()
        {
            var allergy1 = BuildMicrotestAllergy(0, "Medium", "Allergy A", "2019-03-27");
            var allergy2 = BuildMicrotestAllergy(1, "Severe", "Allergy B", "2019-04-27");
            var allergy3 = BuildMicrotestAllergy(2, "Low", "Allergy C", "2019-04-26");

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
            result.Allergies.HasUndeterminedAccess.Should().BeFalse();

            result.Allergies.Data.ElementAt(0).Should().BeEquivalentTo(BuildAllergyItem(allergy2));
            result.Allergies.Data.ElementAt(1).Should().BeEquivalentTo(BuildAllergyItem(allergy3));
            result.Allergies.Data.ElementAt(2).Should().BeEquivalentTo(BuildAllergyItem(allergy1));
        }
        
        [TestMethod] 
        public void MapPatientRecordGetResponse_MapEncounterSuccessfully()
        {
            //Arrange
            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>
                    {
                        BuildMicrotestEncounter(
                            "O/E – Systolic BP reading",
                            "2019-05-02", 
                            "No Units Recorded", 
                            "120" )
                    }
                },
            };
            
            var expectedResult = new MyRecordResponse
            {
                Encounters = new Encounters
                {
                    Data = new List<EncounterItem>
                    { 
                        BuildEncounterItem(item.EncounterData.Encounters.ElementAt(0))
                    }
                }
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.Encounters.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Encounters.Should().BeEquivalentTo(expectedResult.Encounters);
            result.Encounters.HasUndeterminedAccess.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void MapPatientRecordGetResponse_ShouldNotMapEncounterWhenDescriptionIsNullOrEmpty(string description)
        {
            //Arrange
            var encounter1 = BuildMicrotestEncounter(description, "2019-05-02", "120", "No unit recorded");
            var encounter2 = BuildMicrotestEncounter("O/E - Systolic BP reading", "2019-05-02", "120", "No unit recorded");
                
            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData()
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>
                    {
                        encounter1, encounter2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Encounters.Data.Should().HaveCount(1);
            result.Encounters.Data.ElementAt(0).Should().BeEquivalentTo(BuildEncounterItem(encounter2));
            result.Encounters.HasUndeterminedAccess.Should().BeFalse();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DetailedRecordAccessShouldBeFalseWhenNoEncountersReturned()
        {
            //Arrange
            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData()
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>()
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Encounters.HasUndeterminedAccess.Should().BeTrue();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DetailedRecordAccessShouldBeFalseWhenEncountersAreAllFilteredOut()
        {
            //Arrange
            var encounter1 = BuildMicrotestEncounter("", "2019-05-02", "120", "No unit recorded");
            var encounter2 = BuildMicrotestEncounter(null,  "2019-05-02", "120", "No unit recorded");
            
            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData()
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>
                    {
                        encounter1, encounter2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Encounters.Data.Should().HaveCount(0);
            result.Encounters.HasUndeterminedAccess.Should().BeFalse();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderEncountersInDescendingOrder()
        {
            //Arrange
            var encounter1 = BuildMicrotestEncounter("O/E - Systolic BP reading","2017-01-02", "120", "No unit recorded");
            var encounter2 = BuildMicrotestEncounter("O/E - Systolic BP reading","2019-05-20", "110", "No unit recorded");
            var encounter3 = BuildMicrotestEncounter("O/E - Systolic BP reading","2018-10-28", "100", "No unit recorded");
            
            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>
                    {
                        encounter1, encounter2, encounter3,
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Encounters.Data.Should().HaveCount(3);
            result.Encounters.HasUndeterminedAccess.Should().BeFalse();
            result.Encounters.Data.ElementAt(0).Should().BeEquivalentTo(BuildEncounterItem(encounter2));
            result.Encounters.Data.ElementAt(1).Should().BeEquivalentTo(BuildEncounterItem(encounter3));
            result.Encounters.Data.ElementAt(2).Should().BeEquivalentTo(BuildEncounterItem(encounter1));
        }
        
        [DataTestMethod]
        [DataRow("")]
        [DataRow("word")]
        public void MapPatientRecordGetResponse_ShouldDisplayUnknownIfEncountersRecordedOnDateIsInvalidFormat(string recordedOnDate)
        {
            //Arrange
            var encounter1 = BuildMicrotestEncounter("O/E - Systolic BP reading",recordedOnDate, "120", "No unit recorded");
            var encounter2 = BuildMicrotestEncounter("O/E - Systolic BP reading","2019-05-20", "110", "No unit recorded");

            var item = new PatientRecordGetResponse
            {
                EncounterData = new EncounterData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Encounters = new List<Encounter>
                    {
                        encounter1, encounter2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Encounters.Data.Should().HaveCount(2);
            result.Encounters.HasUndeterminedAccess.Should().BeFalse();
            result.Encounters.Data.ElementAt(0).RecordedOn.DatePart.Should().BeEquivalentTo("Unknown");
            result.Encounters.Data.ElementAt(1).RecordedOn.DatePart.Should().BeEquivalentTo(BuildEncounterItem(encounter2).RecordedOn.DatePart);
        }
        
        [TestMethod] 
        public void MapPatientRecordGetResponse_MapReferralSuccessfully()
        {
            //Arrange
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>
                    {
                        BuildMicrotestReferral(
                            "2019-10-10",
                            "Blood chemistry", 
                            "Refer to chiropodist", 
                            "None" )
                    }
                },
            };
            
            //Expected Result
            var expectedResult = new MyRecordResponse
            {
                Referrals = new Referrals
                {
                    Data = new List<ReferralItem>
                    {
                        BuildReferralItem(item.ReferralData.Referrals.ElementAt(0))
                    }
                }
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.Referrals.Data.Should().HaveCount(1);
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Referrals.Should().BeEquivalentTo(expectedResult.Referrals);
            result.Referrals.HasUndeterminedAccess.Should().BeFalse();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void MapPatientRecordGetResponse_ShouldNotMapReferralWhenDescriptionIsNullOrEmpty(string description)
        {
            //Arrange
            var referral1 = BuildMicrotestReferral("2019-10-10", description, "Refer to chiropodist", "None");
            var referral2 = BuildMicrotestReferral("2019-08-20", "Occupations", "Referred to vascular surgeon", "None");
                
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>
                    {
                        referral1, referral2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Referrals.Data.Should().HaveCount(1);
            result.Referrals.Data.ElementAt(0).Should().BeEquivalentTo(BuildReferralItem(referral2));
            result.Referrals.HasUndeterminedAccess.Should().BeFalse();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DetailedRecordAccessShouldBeFalseWhenNoReferralsReturned()
        {
            //Arrange
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>()
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Referrals.HasUndeterminedAccess.Should().BeTrue();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DetailedRecordAccessShouldBeFalseWhenReferralsAreAllFilteredOut()
        {
            //Arrange
            var referral1 = BuildMicrotestReferral("2019-05-02", "", "Chiropody", "None");
            var referral2 = BuildMicrotestReferral("2019-05-02",  null, "Occupations", "None");
            
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>
                    {
                        referral1, referral2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Referrals.Data.Should().HaveCount(0);
            result.Referrals.HasUndeterminedAccess.Should().BeFalse();
        }
        
           
        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderReferralsInDescendingOrder()
        {
            //Arrange
            var referral1 = BuildMicrotestReferral("2017-01-02","Blood chemistry 1", "Refer to chiropodist", "None");
            var referral2 = BuildMicrotestReferral("2019-05-20","Occupations", "Refer to vascular surgeon", "None");
            var referral3 = BuildMicrotestReferral("2018-10-28","Blood chemistry 2", "Refer to chiropodist", "None");
            
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>
                    {
                        referral1, referral2, referral3,
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Referrals.Data.Should().HaveCount(3);
            result.Referrals.HasUndeterminedAccess.Should().BeFalse();
            result.Referrals.Data.ElementAt(0).Should().BeEquivalentTo(BuildReferralItem(referral2));
            result.Referrals.Data.ElementAt(1).Should().BeEquivalentTo(BuildReferralItem(referral3));
            result.Referrals.Data.ElementAt(2).Should().BeEquivalentTo(BuildReferralItem(referral1));
        }
        
        [DataTestMethod]
        [DataRow("")]
        [DataRow("word")]
        public void MapPatientRecordGetResponse_ShouldDisplayUnknownIfReferralsDateIsInvalidFormat(string recordDate)
        {
            //Arrange
            var referral1 = BuildMicrotestReferral(recordDate,"Occupations", "Refer to vascular surgeon", "None");
            var referral2 = BuildMicrotestReferral("2018-10-28","Blood chemistry 2", "Refer to chiropodist", "None");
            
            var item = new PatientRecordGetResponse
            {
                ReferralData = new ReferralData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Referrals = new List<Referral>
                    {
                        referral1, referral2
                    }
                },
            };

            //Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Referrals.Data.Should().HaveCount(2);
            result.Referrals.HasUndeterminedAccess.Should().BeFalse();
            result.Referrals.Data.ElementAt(0).RecordDate.DatePart.Should().BeEquivalentTo(BuildReferralItem(referral2).RecordDate.DatePart);
            result.Referrals.Data.ElementAt(1).RecordDate.DatePart.Should().BeEquivalentTo("Unknown");
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapSingleMedicationPerSection()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Data.AcuteMedications.Should().HaveCount(1);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(1);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(1);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapCurrentMedicationsOnly()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(1);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapHistoricMedicationsOnly()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(1);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapAcuteMedicationsOnly()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Data.AcuteMedications.Should().HaveCount(1);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }


        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMultipleMedicationsForEachSection()
        {
            // Arrange
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
                        },
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeTrue();
            result.Medications.Data.AcuteMedications.Should().HaveCount(4);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(3);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(2);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
            result.Medications.Should().BeEquivalentTo(expectedResult.Medications);
        }


        [DataTestMethod]
        [DataRow(null)]
        [DataRow("no last prescribed date")]
        [DataRow("")]
        public void MapPatientRecordGetResponse_ShouldNotMapMedicationItemsWithInvalidDates(String prescribedDate)
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeFalse();
            result.Medications.Data.AcuteMedications.Should().HaveCount(0);
            result.Medications.Data.CurrentRepeatMedications.Should().HaveCount(0);
            result.Medications.Data.DiscontinuedRepeatMedications.Should().HaveCount(0);
            result.Medications.HasUndeterminedAccess.Should().BeFalse();
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

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.HasSummaryRecordAccess.Should().BeFalse();
            result.Medications.HasUndeterminedAccess.Should().BeTrue();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapSingleImmunisationItem()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.Should().HaveCount(1);
            result.Immunisations.HasUndeterminedAccess.Should().BeFalse();

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
            // Arrange
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

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.Should().HaveCount(2);
            result.Immunisations.HasUndeterminedAccess.Should().BeFalse();
            result.Immunisations.Data.ElementAt(0).Should().BeEquivalentTo(BuildImmunisationItem(imm2));
        }


        [DataTestMethod]
        [DataRow(null)]
        [DataRow("no next date available")]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapImmunisationsWithUnparsableNextDate(string date)
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.Should().HaveCount(1);
            result.Immunisations.HasUndeterminedAccess.Should().BeFalse();
            result.Immunisations.Data.ElementAt(0).Should().BeEquivalentTo(BuildImmunisationItem(imm1));
        }



        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderImmunisationsByDateDescending()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(item);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Immunisations.Data.Should().HaveCount(3);
            result.Immunisations.HasUndeterminedAccess.Should().BeFalse();

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
            result.Immunisations.HasUndeterminedAccess.Should().BeTrue();
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_WillMapProblemAndUseRawFinishDateValue_WhenFinishDateCannotBeParsed()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Data.Should().HaveCount(1);
            result.Problems.HasUndeterminedAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_CanSuccessfullyMapProblemsWhenFinishDateCanBeParsed()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Data.Should().HaveCount(1);
            result.Problems.HasUndeterminedAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapProblemItemEvenWhenStartDateCannotBeParsed()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Data.Should().HaveCount(1);
            result.Problems.HasUndeterminedAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("No rubric recorded")]
        [DataRow(" No rubric recorded ")]       //spaces at start and end of string
        [DataRow("no RUBrIc RECOrded")]         //case insensitive check
        [DataRow("   NO rubric RECOrdED    ")]  //case insensitive check with leading and trailing spaces
        public void MapPatientRecordGetResponse_ShouldNotMapProblemItemWhenRubricValueIsNotValid(string rubric)
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.Problems.Data.Should().HaveCount(0);
            result.Problems.HasUndeterminedAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderProblemsByStartDateDescending()
        {
            // Arrange
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

            // Act
            var result = _mapper.Map(microtestRecordResponse);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.Problems.Data.Should().HaveCount(5);
            result.Problems.HasUndeterminedAccess.Should().BeFalse();
            result.Problems.Should().BeEquivalentTo(expectedResult.Problems);
        }


        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapInrResultsAndPathResultsSuccessfully()
        {
            //Arrange
            var inrResult = BuildMicrotestInrTestResult("2019-08-28", "Atrial fibrillation", "ECG", "2.5", "2.4","10mg/day", "1 Sept 2019");
            var pathResult = BuildMicrotestPathTestResult("GFR", "2019-08-28", "Status 1", "GFR calculated", "None", "No units");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
               TestResultData = BuildMicrotestTestResult(new List<InrResult> {inrResult}, new List<PathResult> {pathResult})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>
                    {
                        BuildTestResultItem(inrResult),
                        BuildTestResultItem(pathResult)
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.TestResults.Data.Should().HaveCount(2);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
            result.TestResults.Should().BeEquivalentTo(expectedResult.TestResults);
        }


        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapInrResultsOnlySuccessfully()
        {
            //Arrange
            var inrResult = BuildMicrotestInrTestResult("2019-08-28", "Atrial fibrillation", "ECG", "2.5", "2.4","10mg/day", "1 Sept 2019");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(new List<InrResult> {inrResult}, new List<PathResult> {})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>
                    {
                        BuildTestResultItem(inrResult),
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.TestResults.Data.Should().HaveCount(1);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
            result.TestResults.Should().BeEquivalentTo(expectedResult.TestResults);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapPathResultsOnlySuccessfully()
        {
            //Arrange
            var pathResult = BuildMicrotestPathTestResult("GFR", "2019-08-28", "Status 1", "GFR calculated", "None", "No units");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(new List<InrResult> {}, new List<PathResult> {pathResult})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>
                    {
                        BuildTestResultItem(pathResult)
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.TestResults.Data.Should().HaveCount(1);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
            result.TestResults.Should().BeEquivalentTo(expectedResult.TestResults);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldMapNoTestResultsSuccessfully()
        {
            //Arrange
            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(new List<InrResult>(), new List<PathResult>())
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>()
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.TestResults.Data.Should().HaveCount(0);
            result.TestResults.HasUndeterminedAccess.Should().BeTrue();
        }


        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldOrderTestResultsFirstByInrDescendingThenPathDescending()
        {
            //Arrange
            var inrResult1 = BuildMicrotestInrTestResult("2019-01-02", "Atrial fibrillation", "ECG", "2", "2.4","2mg/wk", "13 Sept 2019");
            var inrResult2 = BuildMicrotestInrTestResult("2019-01-03", "Blackouts", "XYZ", "3", "3.4","10mg/day", "1 Sept 2019");
            var pathResult1 = BuildMicrotestPathTestResult("ABC", "2019-01-01", "Status 1", "ABC calculated", "2", "6");
            var pathResult2 = BuildMicrotestPathTestResult("GFR", "2019-01-04", "Status 2", "GFR calculated", "3", "1");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(
                    new List<InrResult> {inrResult1, inrResult2}, new List<PathResult> {pathResult1, pathResult2})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>
                    {
                        BuildTestResultItem(inrResult2),
                        BuildTestResultItem(inrResult1),
                        BuildTestResultItem(pathResult2),
                        BuildTestResultItem(pathResult1),
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.TestResults.Data.Should().HaveCount(4);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
            result.TestResults.Should().BeEquivalentTo(expectedResult.TestResults, o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldFilterOutPathResultsWithInvalidStatus()
        {
            //Arrange
            var pathResult1 = BuildMicrotestPathTestResult("ABC", "2019-01-01", TestResultStatus.AwaitingResults, "ABC calculated", "2", "6");
            var pathResult2 = BuildMicrotestPathTestResult("GFR", "2019-01-04", TestResultStatus.AwaitingResults, "GFR calculated", "3", "1");
            var pathResult3 = BuildMicrotestPathTestResult("GFR", "2019-01-04", "Good Status", "GFR calculated", "3", "1");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(
                    new List<InrResult>(), new List<PathResult> {pathResult1, pathResult2, pathResult3})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults
                {
                    Data = new List<TestResultItem>
                    {
                        BuildTestResultItem(pathResult2),
                    }
                }
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeTrue();
            result.TestResults.Data.Should().HaveCount(1);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
            result.TestResults.Should().BeEquivalentTo(expectedResult.TestResults);
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_ShouldNotSetUndeterminedAccessTrueWhenAllTestResultsFilteredOut()
        {
            //Arrange
            var pathResult1 = BuildMicrotestPathTestResult("ABC", "2019-01-01", TestResultStatus.AwaitingResults, "ABC calculated", "2", "6");
            var pathResult2 = BuildMicrotestPathTestResult("GFR", "2019-01-04", TestResultStatus.AwaitingResults, "GFR calculated", "3", "1");

            var microtestRecordResponse = new PatientRecordGetResponse
            {
                TestResultData = BuildMicrotestTestResult(
                    new List<InrResult>(), new List<PathResult> {pathResult1, pathResult2})
            };

            //Expect
            var expectedResult = new MyRecordResponse
            {
                TestResults = new TestResults()
            };

            //Act
            var result = _mapper.Map(microtestRecordResponse);

            //Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.TestResults.Data.Should().HaveCount(0);
            result.TestResults.HasUndeterminedAccess.Should().BeFalse();
        }


        
        [TestMethod]
        public void MapPatientRecordGetResponse_MapOrderedMedicalHistoryWhenReturnsSuccessfully()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                MedicalHistoryData = new MedicalHistoryData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    MedicalHistories = new List<MedicalHistory>
                    {
                        BuildMicrotestMedicalHistory(
                            "2019-08-07", "test vaccination A", "test desc A"),
                        BuildMicrotestMedicalHistory(
                            "2017-08-07", "test vaccination C", "test desc C"),
                        BuildMicrotestMedicalHistory(
                            "2018-08-07", "test vaccination B", "test desc B"),
                    }
                }
            };

            var expectedResult = new MyRecordResponse
            {
                MedicalHistories = new MedicalHistories
                {
                    Data = new List<MedicalHistoryItem>
                    {
                        BuildMedicalHistoryItem(item.MedicalHistoryData.MedicalHistories.ElementAt(0)),
                        BuildMedicalHistoryItem(item.MedicalHistoryData.MedicalHistories.ElementAt(2)),
                        BuildMedicalHistoryItem(item.MedicalHistoryData.MedicalHistories.ElementAt(1))
                    }
                }
            };
            
            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.MedicalHistories.Data.Should().HaveCount(3);
            result.MedicalHistories.Should().BeEquivalentTo(expectedResult.MedicalHistories, m => m.WithStrictOrdering());
            result.MedicalHistories.HasUndeterminedAccess.Should().BeFalse();
            result.HasDetailedRecordAccess.Should().BeTrue();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DoesntMapMedicalHistoryWhenRubricIsEmpty()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                MedicalHistoryData = new MedicalHistoryData
                {
                    Count = 3,
                    HasAccess = true,
                    HasErrored = false,
                    MedicalHistories = new List<MedicalHistory>
                    {
                        BuildMicrotestMedicalHistory(
                            "2019-08-07", "", "test desc with no rubric"),
                        BuildMicrotestMedicalHistory(
                            "2017-08-07", "test vaccination A", "test desc A"),
                        BuildMicrotestMedicalHistory(
                            "2018-08-07", "test vaccination B", "test desc B"),
                    }
                }
            };

            var expectedResult = new MyRecordResponse
            {
                MedicalHistories = new MedicalHistories
                {
                    Data = new List<MedicalHistoryItem>
                    {
                        BuildMedicalHistoryItem(item.MedicalHistoryData.MedicalHistories.ElementAt(2)),
                        BuildMedicalHistoryItem(item.MedicalHistoryData.MedicalHistories.ElementAt(1))                    
                    }
                }
            };
            
            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.MedicalHistories.Data.Should().HaveCount(2);
            result.MedicalHistories.Should().BeEquivalentTo(expectedResult.MedicalHistories, m => m.WithStrictOrdering());
            result.MedicalHistories.HasUndeterminedAccess.Should().BeFalse();
            result.HasDetailedRecordAccess.Should().BeTrue();
        }
        
        [TestMethod]
        public void MapPatientRecordGetResponse_DoesntMapMedicalHistoryWhenAllRubricAreEmpty()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                MedicalHistoryData = new MedicalHistoryData
                {
                    Count = 2,
                    HasAccess = true,
                    HasErrored = false,
                    MedicalHistories = new List<MedicalHistory>
                    {
                        BuildMicrotestMedicalHistory(
                            "2019-08-07", "", "test desc with no rubric"),
                        BuildMicrotestMedicalHistory(
                            "2017-08-07", "", "test desc with no rubric")
                    }
                }
            };
            
            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.MedicalHistories.Data.Should().HaveCount(0);
            result.MedicalHistories.HasUndeterminedAccess.Should().BeFalse();
            result.HasDetailedRecordAccess.Should().BeFalse();
        }
        
        
        [TestMethod]
        public void MapPatientRecordGetResponse_MapMedicalHistoryWhenNoDataIsReturned()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                MedicalHistoryData = new MedicalHistoryData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    MedicalHistories = new List<MedicalHistory>()
                },
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.HasDetailedRecordAccess.Should().BeFalse();
            result.MedicalHistories.HasUndeterminedAccess.Should().BeTrue();
        }

        [TestMethod]
        public void MapPatientRecordGetResponse_DetailedRecordAccessShouldBeFalseWhenNoRecallsReturned()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                RecallData = new RecallData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Recalls = new List<Recall>()
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Recalls.Data.Should().HaveCount(0);
            result.Recalls.HasUndeterminedAccess.Should().BeTrue();
            result.HasDetailedRecordAccess.Should().BeFalse();
        } 
        
        [TestMethod]
        public void MapPatientRecordGetResponse_MapSingleRecallCorrectly()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                RecallData = new RecallData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Recalls = new List<Recall>
                    {
                        BuildMicrotestRecall("2019-03-27", "name", "desc", "Nut Allergy", "2019-03-27", "status"),
                    }
                },
            };
            
            var expectedResult = new MyRecordResponse
            {
                Recalls = new Recalls
                {
                    Data = new List<RecallItem>
                    {
                        BuildRecallItem(item.RecallData.Recalls.ElementAt(0))
                    }
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Recalls.Data.Should().HaveCount(1);
            result.Recalls.Should().BeEquivalentTo(expectedResult.Recalls);
            result.Recalls.HasUndeterminedAccess.Should().BeFalse();
            result.HasDetailedRecordAccess.Should().BeTrue();
        }
        
        
        [TestMethod]
        public void MapPatientRecordGetResponse_MapMultipleRecordsOrderedCorrectly()
        {
            // Arrange
            var item = new PatientRecordGetResponse
            {
                RecallData = new RecallData
                {
                    Count = 0,
                    HasAccess = true,
                    HasErrored = false,
                    Recalls = new List<Recall>
                    {
                        BuildMicrotestRecall("2019-02-27", "name 1", "desc 1", "result 1", "2019-03-27", "status 1"),
                        BuildMicrotestRecall("2019-01-27", "name 2", "desc 2", "result 2", "2019-03-28", "status 2"),
                        BuildMicrotestRecall("2019-03-27", "name 3", "desc 3", "result 3", "2019-03-29", "status 3")
                    }
                },
            };
            
            var expectedResult = new MyRecordResponse
            {
                Recalls = new Recalls
                {
                    Data = new List<RecallItem>
                    {
                        BuildRecallItem(item.RecallData.Recalls.ElementAt(2)),
                        BuildRecallItem(item.RecallData.Recalls.ElementAt(0)),
                        BuildRecallItem(item.RecallData.Recalls.ElementAt(1))
                    }
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Recalls.Data.Should().HaveCount(3);
            result.Recalls.Should().BeEquivalentTo(expectedResult.Recalls,r => r.WithStrictOrdering());;
            result.Recalls.HasUndeterminedAccess.Should().BeFalse();
            result.HasDetailedRecordAccess.Should().BeTrue();
        }
        
        
        
        private static MedicalHistoryItem BuildMedicalHistoryItem(MedicalHistory medicalHistory)
        {
            return new MedicalHistoryItem
            {
                StartDate = GetMyRecordDate(medicalHistory.StartDate),
                Rubric = medicalHistory.Rubric,
                Description = medicalHistory.Description
            };
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

        private static EncounterItem BuildEncounterItem(Encounter encounter)
        {
            return new EncounterItem
            {
                Description = encounter.Description,
                Unit = encounter.Unit,
                Value = encounter.Value,
                RecordedOn = new MyRecordDate
                {
                    Value = DateTime.TryParse(encounter.RecordedOn, out var encounterDate)
                        ? encounterDate
                        : (DateTimeOffset?) null,
                    DatePart = "Unknown"  
                }
            };
        }

        private static ReferralItem BuildReferralItem(Referral referral)
        {
            return new ReferralItem
            {
                Description = referral.Description,
                Speciality = referral.Speciality,
                Ubrn = referral.Ubrn,
                RecordDate = new MyRecordDate
                {
                    Value = DateTime.TryParse(referral.RecordDate, out var referralDate)
                        ? referralDate
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
                new MedicationLineItem { Text = $"Reason: {item.Reason}" }
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
                    new ProblemLineItem { Text = $"Finish Date: {finishDate}" },
                    new ProblemLineItem { Text = rubric }
                }
            };
        }


       private static RecallItem BuildRecallItem(Recall recall)
       {
           return new RecallItem
           {
               RecordDate = recall.RecordDate != null
                   ? new MyRecordDate
                   {
                       Value = DateTime.TryParse(recall.RecordDate, out var recordDate)
                           ? recordDate
                           : (DateTimeOffset?) null,
                       DatePart = "Unknown"
                   }
                   : null,
               Name = recall.Name,
               Description = recall.Description,
               Result = recall.Result,
               NextDate = recall.NextDate,
               Status = recall.Status
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


        private static TestResultItem BuildTestResultItem(InrResult inrResult)
        {
            return new TestResultItem
            {
                Date = new MyRecordDate
                {
                    Value = DateTime.TryParse(inrResult.RecordDateTime, out var recordDateTime)
                        ? recordDateTime
                        : (DateTimeOffset?) null,
                    DatePart = "Unknown"
                },
                AssociatedTexts = new List<string>
                {
                    $"INR Results: {inrResult.Value} (target - {inrResult.Target})",
                    $"Condition: {inrResult.CodeDescription}",
                    $"Therapy: {inrResult.Therapy}",
                    $"Dose: {inrResult.Dose}",
                    $"Next test date: {inrResult.NextTestDate}"
                }
            };
        }

        private static TestResultItem BuildTestResultItem(PathResult pathResult)
        {
            return new TestResultItem
            {
                Date = new MyRecordDate
                {
                    Value = DateTime.TryParse(pathResult.RecordDate, out var recordDate)
                        ? recordDate
                        : (DateTimeOffset?) null,
                    DatePart = "Unknown"
                },
                AssociatedTexts = new List<string>
                {
                    $"{pathResult.Name}: {pathResult.ElementName}",
                    $"Value: {pathResult.Value}",
                    $"Units: {pathResult.Units}"
                }
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

        private static Encounter BuildMicrotestEncounter(string description, string recordedOn, string unit, string value)
        {
            return new Encounter
            {
                Description = description,
                RecordedOn = recordedOn,
                Unit = unit,
                Value = value
            };
        }
        
        private static Referral BuildMicrotestReferral(string recordDate, string description, string speciality, string ubrn)
        {
            return new Referral
            {
                RecordDate = recordDate,
                Description = description,
                Speciality = speciality,
                Ubrn = ubrn
            };
        }
        
        private static MedicalHistory BuildMicrotestMedicalHistory(string startDate, string rubric, string desc)
        {
            return new MedicalHistory()
            {
                Description = desc,
                StartDate = startDate,
                Rubric = rubric
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

        private static InrResult BuildMicrotestInrTestResult(string recordDateTime, string code, string therapy,
            string target, string value, string dose, string nextTestDate)
        {
            return new InrResult
            {
                RecordDateTime = recordDateTime,
                CodeDescription = code,
                Therapy = therapy,
                Target = target,
                Value = value,
                Dose = dose,
                NextTestDate = nextTestDate
            };
        }

        private static PathResult BuildMicrotestPathTestResult(string name, string recordDate, string status,
            string elementName, string value, string units)
        {
            return new PathResult
            {
                Name = name,
                RecordDate = recordDate,
                Status = status,
                ElementName = elementName,
                Value = value,
                Units = units
            };
        }

        private static TestResultData BuildMicrotestTestResult(List<InrResult> inrResults, List<PathResult> pathResults)
        {
            return new TestResultData
            {
                Count = 2,
                HasAccess = true,
                HasErrored = false,
                TestResult = new GpSystems.Suppliers.Microtest.Models.PatientRecord.TestResult
                {
                    InrResultsData = new InrResultData
                    {
                        Count = inrResults.Count,
                        InrResults = inrResults
                    },
                    PathResultsData = new PathResultData
                    {
                        Count = pathResults.Count,
                        PathResults = pathResults
                    }
                }
            };
        }

        private static Recall BuildMicrotestRecall
            (string recordDate, string name, string description, string result, string nextDate, string status)
        {
            return new Recall
            {
                RecordDate = recordDate,
                Name = name,
                Description = description,
                NextDate = nextDate,
                Result = result,
                Status = status
            };
        }
    }

}