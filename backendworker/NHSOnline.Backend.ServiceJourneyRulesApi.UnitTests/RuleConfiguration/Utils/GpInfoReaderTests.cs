using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class GpInfoReaderTests
    {
        private const string GpInfoFilePath = "TestData/GpInfo/gpinfo.csv";
        private IGpInfoReader _gpInfoReader;
        private Mock<ILogger<GpInfoReader>> _mockLogger;
        private Mock<IFileHandler> _mockFileHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = fixture.Freeze<Mock<ILogger<GpInfoReader>>>();
            _mockFileHandler = fixture.Freeze<Mock<IFileHandler>>();
            _gpInfoReader = fixture.Create<GpInfoReader>();
        }

        [TestMethod]
        public void GetGpInfo_WhenCalledWithValidFileName_MapAllFieldsForGivenOdsCode()
        {
            // Arrange
            var expected = new GpInfo
            {
                Ods = "A81001",
                CcgCode = "00K",
                Supplier = GpInfoSupplier.Tpp,
                EndpointCreated = "20160125"
            };

            SetupFileHandlerForMultipleOdsCode();

            // Act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);

            // Assert
            result.Should().NotBeNull().And.ContainKey(expected.Ods);
            result[expected.Ods].Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        [DataRow("A81001", "20160125")]
        [DataRow("M81001", "20180121")]
        [DataRow("X1001", "20180921")]
        public void GetGpInfo_WhenCalledWithValidFileName_ReturnsLatestRecordForGivenOdsCode(string odsCode,
            string endpointCreated)
        {
            // Arrange            
            SetupFileHandlerForMultipleOdsCode();

            // Act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);

            // Assert
            result.Should().ContainKey(odsCode);
            result[odsCode].EndpointCreated.Should().Be(endpointCreated);
        }

        [TestMethod]
        [DataRow("EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)", GpInfoSupplier.Emis)]
        [DataRow("THE PHOENIX PARTNERSHIP", GpInfoSupplier.Tpp)]
        [DataRow("IN PRACTICE SYSTEMS LTD", GpInfoSupplier.Vision)]
        [DataRow("MICROTEST LTD", GpInfoSupplier.Microtest)]
        [DataRow("Test", GpInfoSupplier.Unknown)]
        public void GetGpInfo_WhenCalledWithValidFileName_MatchesGpSupplierEnumValue
            (string gpSupplier, GpInfoSupplier expectedSupplierEnumValue)
        {
            // Arrange         
            SetupFileHandlerForVariousSuppliers(gpSupplier);

            // Act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);

            // Assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Value.Supplier.Should().Be(expectedSupplierEnumValue);
        }

        [TestMethod]
        public void GetGpInfo_WhenCalledWithUnknownFile_LogsErrorAndReturnNull()
        {
            // Arrange
            _mockFileHandler.Setup(f => f.GetTextReader(It.IsAny<string>()))
                .Throws<FileNotFoundException>();

            // Act
            var result = _gpInfoReader.GetGpInfo("UnknownFile.csv");

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetGpInfo_WhenCalledWithInvalidCsv_LogsErrorAndReturnNull()
        {
            // Arrange
            const string filePath = "InvalidGpInfo.csv";
            var fileDataStream = new StringReader
            (
                "ODS,Organisation,ClosedDate,PartyKey,ASID,Supplier,Product,Version,EndpointCreated,CCGCode,CCG\n" +
                "X1001,XYZ SURGERY,2018-06-21,YGA-0021074");

            _mockFileHandler.Setup(erh => erh.GetTextReader(filePath))
                .Returns(fileDataStream);

            // Act
            var result = _gpInfoReader.GetGpInfo("InvalidGpInfo.csv");

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());

            result.Should().BeNull();
        }

        private void SetupFileHandlerForVariousSuppliers(string gpSupplier)
        {
            var fileDataStream = new StringReader(
                "ODS,Organisation,ClosedDate,PartyKey,ASID,Supplier,Product,Version,EndpointCreated,CCGCode,CCG\n" +
                "A81001,THE DENSHAM SURGERY,,YGA-0021074,2.86E+11," + gpSupplier +
                ",SystmOne,601 Core GP2GPLM EPS2(AS),20140121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG");

            _mockFileHandler.Setup(erh => erh.GetTextReader(GpInfoFilePath))
                .Returns(fileDataStream);
        }

        private void SetupFileHandlerForMultipleOdsCode()
        {
            var fileDataStream = new StringReader
            (
                "ODS,Organisation,ClosedDate,PartyKey,ASID,Supplier,Product,Version,EndpointCreated,CCGCode,CCG\n" +
                "X1001,XYZ SURGERY,2018-06-21,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20180621,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "X1001,XYZ SURGERY,,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20180921,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "M81001,MXYZ SURGERY,,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20170121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "M81001,MXYZ SURGERY,,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20180121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "A81001,THE DENSHAM SURGERY,,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20140121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "A81001,THE DENSHAM SURGERY,,YGA-0021074,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20150121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG\n" +
                "A81001,THE DENSHAM SURGERY,2016-01-25,YGA-0021075,2.86E+11,THE PHOENIX PARTNERSHIP,SystmOne,601 Core GP2GPLM EPS2(AS),20160125,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG");

            _mockFileHandler.Setup(erh => erh.GetTextReader(GpInfoFilePath))
                .Returns(fileDataStream);
        }
    }
}