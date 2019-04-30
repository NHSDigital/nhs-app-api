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
            //arrange
            var expected = new GpInfo()
            {
                Ods = "A81001",
                CcgCode = "00K",
                Supplier = GpInfoSupplier.Tpp,
                EndpointCreated = "20160125"
            };
            
            SetupFileHandlerForMultipleOdsCode();
            
            // act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);
            
            //assert
            result.Should().NotBeNull().And.ContainKey(expected.Ods);
            result[expected.Ods].Should().BeEquivalentTo(expected);
        }
        
        [TestMethod]
        [DataRow("A81001", "20160125")]
        [DataRow("M81001", "20180121")]
        [DataRow("X1001", "20180921")]
        public void GetGpInfo_WhenCalledWithValidFileName_ReturnsLatestRecordForGivenOdsCode(string odsCode, string EndpointCreated)
        {
            //arrange            
            SetupFileHandlerForMultipleOdsCode();
            
            // act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);
            
            // assert
            result.Should().ContainKey(odsCode);
            result[odsCode].EndpointCreated.Should().Be(EndpointCreated);
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
            //arrange         
            SetupFileHandlerForVariousSuppliers(gpSupplier);
            
            // act
            var result = _gpInfoReader.GetGpInfo(GpInfoFilePath);
            
            //assert
            result.Should().NotBeNull().And.HaveCount(1);
            result.First().Value.Supplier.Should().Be(expectedSupplierEnumValue);
        }
        
        [TestMethod]
        public void GetGpInfo_WhenCalledWithUnkownFile_LogsErrorAndReturnNull()
        {   
            // arrange
            _mockFileHandler.Setup(f => f.GetTextReaderToReadFileContent(It.IsAny<string>()))
                .Throws<FileNotFoundException>();
            
            // act
            var result = _gpInfoReader.GetGpInfo("UnkownFile.csv");
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());
            
            result.Should().BeNull();
        }
        
        [TestMethod]
        public void GetGpInfo_WhenCalledWithInvalidCsv_LogsErrorAndReturnNull()
        {   
            // arrange
            const string filePath = "InvalidGpInfo.csv";
            var fileDataStream = new StringReader
            (
                "ODS,Organisation,ClosedDate,PartyKey,ASID,Supplier,Product,Version,EndpointCreated,CCGCode,CCG\n" +
                "X1001,XYZ SURGERY,2018-06-21,YGA-0021074");

            _mockFileHandler.Setup(erh => erh.GetTextReaderToReadFileContent(filePath))
                .Returns(fileDataStream);
            
            // act
            var result = _gpInfoReader.GetGpInfo("InvalidGpInfo.csv");
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());
            
            result.Should().BeNull();
        }
        
        private void SetupFileHandlerForVariousSuppliers(string gpSupplier)
        {
            var fileDataStream = new StringReader("ODS,Organisation,ClosedDate,PartyKey,ASID,Supplier,Product,Version,EndpointCreated,CCGCode,CCG\n" +
                                                  "A81001,THE DENSHAM SURGERY,,YGA-0021074,2.86E+11,"+ gpSupplier +",SystmOne,601 Core GP2GPLM EPS2(AS),20140121,00K,NHS HARTLEPOOL AND STOCKTON-ON-TEES CCG");

            _mockFileHandler.Setup(erh => erh.GetTextReaderToReadFileContent(GpInfoFilePath))
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

            _mockFileHandler.Setup(erh => erh.GetTextReaderToReadFileContent(GpInfoFilePath))
                .Returns(fileDataStream);
        }
    }
}