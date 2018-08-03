using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppTestResultsMapperTests
    {
        private ITppMyRecordMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppMyRecordMapper();
        }

        [TestMethod]
        public void MapTestResultsViewReplyToTestResultsResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new TestResultsViewReply();

            // Act
            var tppTestResults = new TppTestResultsMapper().Map(item);
            var result = _mapper.Map(new Allergies(), new Medications(), new TppDcrEvents(), tppTestResults);

            // Assert
            result.Should().NotBeNull();
            result.TestResults.Data.Should().BeEmpty();           
        }

        [TestMethod]
        public void MapTestResultsViewReplyToTestResultsResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var testResultsViewReply = GetTppTestResults();

            var testResults = new List<TestResultItem>
            {
                new TestResultItem
                {
                    
                    Date = new MyRecordDate { Value = DateTimeOffset.Parse(testResultsViewReply.Items[0].Date, CultureInfo.InvariantCulture) },
                    Description = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", testResultsViewReply.Items[0].Description, testResultsViewReply.Items[0].Value)
                },
                new TestResultItem
                {
                    Date = new MyRecordDate { Value = DateTimeOffset.Parse(testResultsViewReply.Items[1].Date, CultureInfo.InvariantCulture) },
                    Description = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", testResultsViewReply.Items[1].Description, testResultsViewReply.Items[1].Value)
                }                
            };
            // Act
            var result = new TppTestResultsMapper().Map(testResultsViewReply);
            
            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(testResultsViewReply.Items.Count());
            result.Data.Should().BeEquivalentTo(testResults);
        }

        private TestResultsViewReply GetTppTestResults()
        {
            return new TestResultsViewReply
            {     
                Items = new List<TestResultsViewReplyItem>
                {
                    new TestResultsViewReplyItem
                    {
                        
                        Date = "2001-06-14T00:00:00.0Z",
                        Description = "Pathology", 
                        Value = "Anticoag Control (Warfarin), Read",
                    },
                    new TestResultsViewReplyItem
                    {                      
                        Date = "2001-06-28T00:00:00.0Z",
                        Description = "Pathology", 
                        Value = "Mic Cult Sens (Urine), Read",
                    },
                }                
            };
        }
    }
}
