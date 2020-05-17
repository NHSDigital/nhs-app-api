using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            _systemUnderTest = new EmisTestResultMapper(Mock.Of<ILogger<EmisTestResultMapper>>());
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullResponse_ThrowsNullReferenceException()
        {
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("testResultRequestsGetResponse");
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullDateValue_GivesATestResultWithEmptyDate()
        {
            // Arrange
            var testResults = new List<TestResult>
            {
                new TestResult
                {
                    Value = new Value
                    {
                        Term = "testTerm",
                        TextValue = "testTextVal",
                        NumericUnits = "cc",
                        EffectiveDate = new EffectiveDate(),
                    }
                }
            };

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

            mappedTestResultsList.Data.Single().Date.Should().NotBeNull();
            mappedTestResultsList.Data.Single().Date.Value.Should().BeNull();
            mappedTestResultsList.Data.Single().Date.DatePart.Should().BeNull();
        }

        [TestMethod]
        public void MapTestResultsRequestsGetResponseToTestResultsListResponse_WithNullDate_GivesATestResultWithEmptyDate()
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

            mappedTestResultsList.Data.Single().Date.Should().NotBeNull();
            mappedTestResultsList.Data.Single().Date.Value.Should().BeNull();
            mappedTestResultsList.Data.Single().Date.DatePart.Should().BeNull();
        }
    }
}