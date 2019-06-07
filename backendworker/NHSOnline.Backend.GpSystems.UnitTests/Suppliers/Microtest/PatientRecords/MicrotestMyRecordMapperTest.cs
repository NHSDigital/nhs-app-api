using System;
using System.Collections.Generic;
using System.Linq;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new MicrotestMyRecordMapper();
        }

        [TestMethod]
        public void MapNullObjectThrowsArgumentNullException()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientRecordGetResponse");
        }

        [TestMethod]
        public void MapSuccess()
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
        public void TestNullOrEmptySeverityNotMapped(string severity)
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
        public void TestNullOrInvalidDate(string date)
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
        private AllergyItem BuildAllergyItem(Allergy allergy)
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
        private Allergy BuildMicrotestAllergy(int id, string severity, string desc, string startDate)
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
    }

}