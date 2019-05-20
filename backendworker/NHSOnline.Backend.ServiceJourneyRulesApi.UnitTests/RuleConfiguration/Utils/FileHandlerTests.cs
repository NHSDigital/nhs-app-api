using System.IO;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class FileHandlerTests
    {
        private const string DuplicateFileName = "duplicate_file.json";
        private const string UniqueFileName = "invalid_schema.json";
        private IFileHandler _fileHandler;
        private Mock<IDirectory> _directory;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            fixture.Inject(Assembly.GetExecutingAssembly());

            _directory = fixture.Freeze<Mock<IDirectory>>();
            _fileHandler = fixture.Create<FileHandler>();
        }

        [TestMethod]
        public void ReadEmbeddedResourceFromFileName_WhenCalledWithValidFileName_ReturnsFileDataWithNoError()
        {
            // act
            var result = _fileHandler.ReadEmbeddedResourceFromFileName(UniqueFileName, out var fileData);

            // assert
            result.Should().BeTrue();
            fileData.Should().NotBeNull();
            fileData.Name.Should().EndWith(UniqueFileName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(DuplicateFileName)]
        public void ReadEmbeddedResourceFromFileName_WhenCalledWithInvalidFileName_ReturnsFileDataWithError(
            string fileName)
        {
            // act
            var result = _fileHandler.ReadEmbeddedResourceFromFileName(fileName, out var fileData);

            // assert
            result.Should().BeFalse();
            fileData.Should().BeNull();
        }

        [TestMethod]
        public void GetTextReader_WhenCalledWithAValidFilePath_ReturnsTextReader()
        {
            // act
            var result = _fileHandler.GetTextReader("TestData/GpInfo/gpinfo.csv");

            //assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<TextReader>();
        }

        [TestMethod]
        public void GetTextWriter_WhenCalledWithAValidFilePath_ReturnsTextWriter()
        {
            // act
            var result = _fileHandler.GetTextWriter("TestData/GpInfo/text.csv");

            //assert
            _directory.Verify(s => s.CreateDirectory("TestData/GpInfo"));

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<TextWriter>();
        }
    }
}