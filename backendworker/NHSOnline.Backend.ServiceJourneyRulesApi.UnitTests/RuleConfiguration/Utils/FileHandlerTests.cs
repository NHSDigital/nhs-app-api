using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class FileHandlerTests
    {
        private const string NonExistentDirectory = "Configurations/Journeys/NonExistentDirectory";
        private const string DuplicateFileName = "duplicate_file.json";
        private const string UniqueFileName = "invalid_schema.json";
        private Assembly _assembly;
        private Mock<IErrorHandler> _errorHandler;
        private IFileHandler _fileHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _errorHandler = new Mock<IErrorHandler>();
            _assembly = Assembly.GetExecutingAssembly();
            _fileHandler = new FileHandler(_errorHandler.Object, _assembly);
        }

        [TestMethod]
        public void ReadEmbeddedResourceFromFileName_WhenCalledWithValidFileName_ReturnsFileDataWithNoError()
        {
            var fileData = _fileHandler.ReadEmbeddedResourceFromFileName(UniqueFileName);

            Assert.IsFalse(fileData.IsError);
            Assert.IsTrue(fileData.Name.EndsWith(UniqueFileName, StringComparison.Ordinal));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(DuplicateFileName)]
        public void ReadEmbeddedResourceFromFileName_WhenCalledWithInvalidFileName_ReturnsFileDataWithError(string fileName)
        {
            var fileData = _fileHandler.ReadEmbeddedResourceFromFileName(fileName);

            Assert.IsTrue(fileData.IsError);
            Assert.IsNotNull(fileData.Error);
        }

        [TestMethod]
        public void ReadContentFilesFromDirectory_WhenCalledWithAValidDirectory_ReturnsAListOfFilesDataWithNoError()
        {
            var directory = $"TestData/{Constants.FolderNames.JourneyConfigurations}";
            var files = _fileHandler.ReadContentFilesFromDirectory(directory);

            Assert.IsNotNull(files);
            Assert.AreEqual(3, files.Count);
            foreach (var file in files)
            {
                Assert.IsFalse(file.IsError);
                Assert.IsTrue(file.Name.Contains(directory, StringComparison.Ordinal));
            }
        }

        [TestMethod]
        public void ReadContentFilesFromDirectory_WhenCalledWithNonExistentDirectory_ReturnsAnEmptyList()
        {
            var files = _fileHandler.ReadContentFilesFromDirectory(NonExistentDirectory);

            Assert.IsNotNull(files);
            Assert.AreEqual(0, files.Count);
        }
    }
}