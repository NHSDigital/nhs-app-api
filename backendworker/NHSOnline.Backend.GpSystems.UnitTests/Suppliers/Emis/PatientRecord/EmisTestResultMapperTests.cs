using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using TestResult = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord.TestResult;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisTestResultMapperTests
    {
        private EmisTestResultMapper _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new EmisTestResultMapper();
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("testResultRequestsGetResponse");
        }
        
        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullDateValue_GivesResponseWithEmptyDate()
        {
            // Arrange
            var testResults = new List<TestResult>();
            testResults.Add(new TestResult
            {
                Value = new Value
                {
                    Term = "testTerm",
                    TextValue = "testTextVal",
                    NumericUnits = "cc",
                    EffectiveDate = new EffectiveDate
                    {
                        Value = null,
                        DatePart = "mm-dd-yyyy"
                    }
                }
            });
            
            // Act
            var mappedTestResultsList = _systemUnderTest.Map(new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    TestResults = testResults,
                }
            });

            // Assert
            mappedTestResultsList.Should().NotBeNull();
            mappedTestResultsList.Data.Count().Should().Be(1);

            var medicalRecordItem = mappedTestResultsList.Data.First();
            medicalRecordItem.Date.Should().BeNull();
            medicalRecordItem.Description.Should().Be("testTerm: testTextVal cc");
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullDate_GivesResponseWithEmptyDate()
        {
            // Arrange
            var testResults = new List<TestResult>();
            testResults.Add(new TestResult
            {
                Value = new Value
                {
                    Term = "testTerm",
                    TextValue = "testTextVal",
                    NumericUnits = "cc",
                    EffectiveDate = null,
                }
            });
            
            // Act
            var mappedTestResultsList = _systemUnderTest.Map(new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    TestResults = testResults,
                }
            });

            // Assert
            mappedTestResultsList.Should().NotBeNull();
            mappedTestResultsList.Data.Count().Should().Be(1);

            var medicalRecordItem = mappedTestResultsList.Data.First();
            medicalRecordItem.Date.Should().BeNull();
            medicalRecordItem.Description.Should().Be("testTerm: testTextVal cc");
        }
    }
}
