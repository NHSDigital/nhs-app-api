using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
        private const string UniqueFileName = "unique_file.json";
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
            // Act
            var result = _fileHandler.ReadEmbeddedResourceFromFileName(UniqueFileName, out var fileData);

            // Assert
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
            // Act
            var result = _fileHandler.ReadEmbeddedResourceFromFileName(fileName, out var fileData);

            // Assert
            result.Should().BeFalse();
            fileData.Should().BeNull();
        }

        [TestMethod]
        public void GetTextReader_WhenCalledWithAValidFilePath_ReturnsTextReader()
        {
            // Act
            var result = _fileHandler.GetTextReader("TestData/GpInfo/gpinfo.csv");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<TextReader>();
        }

        [TestMethod]
        public void GetTextWriter_WhenCalledWithAValidFilePath_ReturnsTextWriter()
        {
            // Act
            var result = _fileHandler.GetTextWriter("TestData/GpInfo/text.csv");

            // Assert
            _directory.Verify(s => s.CreateDirectory(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "TestData\\GpInfo" : "TestData/GpInfo"));

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<TextWriter>();
        }
    }
}