using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDcrEventItemsMapperTests
    {
        private TppDcrEventItemsMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new TppDcrEventItemsMapper();
        }

        [TestMethod]
        public void Map_WithNullItemList_ReturnsEmptyList()
        {
            List<RequestPatientRecordItem> itemList = null;

            var result = _systemUnderTest.Map(itemList);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_WithListOfItems_ReturnsListOfMappedStrings()
        {
            var itemList = new List<RequestPatientRecordItem>
            {
                new RequestPatientRecordItem
                {
                    Details =
                        "Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]",
                    Type = "Medication template"
                },
                new RequestPatientRecordItem
                {
                    Details = "Benzoin tincture - 500 ml - use as directed",
                    Type = "Medication template"
                },
                new RequestPatientRecordItem
                {
                    Details = "(R) Benzoin tincture - 500 ml - use as directed",
                    Type = "Medication"
                }
            };

            List<string> expectedResult = new List<string>()
            {
                "Medication template - Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]",
                "Medication template - Benzoin tincture - 500 ml - use as directed",
                "Medication - (R) Benzoin tincture - 500 ml - use as directed"
            };

            var result = _systemUnderTest.Map(itemList);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void Map_WithListOfEncodedItems_ReturnsListOfDecodedStrings()
        {
            var itemList = new List<RequestPatientRecordItem>
            {
                new RequestPatientRecordItem
                {
                    Details =
                        "Alimemazine 10mg tablets \t- 1 pack of 28 tablet(s) - \t\t[08:00-1][12:00-1][16:00-1][22:00-1]",
                    Type = "Medication template"
                },
                new RequestPatientRecordItem
                {
                    Details = "Benzoin tincture - 500 ml - \t\t\tuse as directed\n",
                    Type = "Medication template"
                },
                new RequestPatientRecordItem
                {
                    Details = "(R) Benzoin tincture\t\t\t - 500 ml - \nuse as directed\n\n",
                    Type = "Medication"
                }
            };

            List<string> expectedResult = new List<string>()
            {
                "Medication template - Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]",
                "Medication template - Benzoin tincture - 500 ml - use as directed",
                "Medication - (R) Benzoin tincture - 500 ml - ; use as directed"
            };

            var result = _systemUnderTest.Map(itemList);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void Map_WithListOfItemsWithNullProperties_ReturnsListOfStrings()
        {
            var itemList = new List<RequestPatientRecordItem>
            {
                new RequestPatientRecordItem
                {
                    Details =
                        "Alimemazine 10mg tablets \t- 1 pack of 28 tablet(s) - \t\t[08:00-1][12:00-1][16:00-1][22:00-1]",
                    Type = null
                },
                new RequestPatientRecordItem
                {
                    Details = null,
                    Type = "Medication template"
                },
                new RequestPatientRecordItem
                {
                    Details = null,
                    Type = null
                }
            };

            List<string> expectedResult = new List<string>()
            {
                " - Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]",
                "Medication template - ",
                " - "
            };

            var result = _systemUnderTest.Map(itemList);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void Map_WithListWithNullItems_ReturnsListOfStrings()
        {
            var itemList = new List<RequestPatientRecordItem>
            {
                null,
                new RequestPatientRecordItem
                {
                    Details = "Benzoin tincture - 500 ml - \t\t\tuse as directed\n",
                    Type = "Medication template"
                },
                null
            };

            List<string> expectedResult = new List<string>()
            {
                "Medication template - Benzoin tincture - 500 ml - use as directed"
            };

            var result = _systemUnderTest.Map(itemList);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}