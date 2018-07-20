using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDetailedTestResultMapperTests
    {
         private ITppDetailedTestResultMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppDetailedTestResultMapper();
        }

        [TestMethod]
        public void MapTestResultsViewReplyToTestResultsResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new TestResultsViewReply();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.TestResult.Should().BeEmpty();           
        }

        [TestMethod]
        public void MapTestResultsViewReplyToTestResultsResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var testResultsViewReply = new TestResultsViewReply
            {
                Items = new List<TestResultsViewReplyItem>
                {
                    new TestResultsViewReplyItem
                    {
                        Value =
                            "&lt;!DOCTYPE HTML PUBLIC &quot;-//W3C//DTD HTML Strict//EN&quot;&gt;&lt;html&gt;&lt;head&gt;" +
                            "&lt;META http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=ISO-8859-1&quot;&gt;" +
                            "&lt;/head&gt;&lt;body&gt;&lt;table&gt;&lt;TR ID=&quot;testResultData&quot;&gt;&lt;TD&gt;Clinician viewed" +
                            "&lt;/TD&gt;&lt;TD&gt;13 Jul 2018&lt;/TD&gt;&lt;/tr&gt;&lt;TR ID=&quot;testResultData&quot;&gt;&lt;" +
                            "TD&gt;Result type&lt;/TD&gt;&lt;TD&gt;Pathology&lt;/TD&gt;&lt;/tr&gt;&lt;TR ID=&quot;" +
                            "testResultData&quot;&gt;&lt;TD&gt;Tests&lt;/TD&gt;&lt;TD&gt;Urine&lt;BR&gt;&lt;/TD&gt;&lt;/tr&gt;&lt;" +
                            "TR ID=&quot;testResultData&quot;&gt;&lt;TD&gt;Filed by&lt;/TD&gt;&lt;TD" +
                            "&gt;Mr General Nhsapp at Kainos GP Demo Unit (Kainos) - 13 Jul 2018 16:45&lt;/TD&gt;&lt;/tr&gt;" +
                            "&lt;TR ID=&quot;testResultData&quot;&gt;&lt;TD&gt;&lt;b&gt;Result&lt;/b&gt;&lt;/TD&gt;&lt;TD" +
                            "&gt;&lt;b&gt;Normal&lt;/b&gt;&lt;/TD&gt;&lt;/tr&gt;&lt;TR ID=&quot;testResultData&quot;" +
                            "&gt;&lt;TD&gt;&lt;b&gt;What you need to do&lt;/b&gt;&lt;/TD&gt;&lt;TD&gt;&lt;b&gt;No Further Action" +
                            "&lt;/b&gt;&lt;/TD&gt;&lt;/tr&gt;&lt;/table&gt;&lt;br/&gt;&lt;/body&gt;&lt;/html&gt;"
                    }
                }
            };
            
            // Act
            var result = new TppDetailedTestResultMapper().Map(testResultsViewReply);
            
            // Assert
            result.Should().NotBeNull();
            result.TestResult.Should().Be(testResultsViewReply.Items[0].Value);
        }
    }
}