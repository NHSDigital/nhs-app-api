using System;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisPatientDocumentMapperTests
    {
        private IFixture _fixture;
        private Mock<ILogger<IEmisPatientDocumentMapper>> _mockLogger;
        private Mock<IEmisDocumentDownloadConverter> _mockEmisDocumentDownloadConverter;
        private EmisPatientDocumentMapper _systemUnderTest;

        private const string DocWithImgWithAltText =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLPJzE1XKC5KtlUqLilNS1NSSMwpsVVyrcgsLsnMSwfxFEpSK0qU7Gz0Ier1wZoBkzfeT0MAAAA=";
        private const string DocWithImgWithoutAltText =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLPJzE1XKC5KtlUqLilNS1Oys9GHiOuDFQEAVGaspysAAAA=";
        private const string DocWithoutImg =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLMpsAvOz01VyC/JSC1SSM7PK0nNK7HRL7Cz0YfI64MVAwCk81DYMwAAAA==";
        private const string DocWithoutImgContent = "<p>Some other content</p>";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();

            _mockLogger = new Mock<ILogger<IEmisPatientDocumentMapper>>();
            _mockEmisDocumentDownloadConverter = new Mock<IEmisDocumentDownloadConverter>();

            _systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer(), _mockEmisDocumentDownloadConverter.Object);
        }

        [TestMethod]
        public void Map_WhenCalled_ThenItSetsIsViewableAndIsDownloadableToTrue()
        {
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            var patientDocument = _systemUnderTest.Map(individualDocument, "jpg", "Blood test results alt text");

            Assert.IsTrue(patientDocument.IsViewable);
            Assert.IsTrue(patientDocument.IsDownloadable);
        }

        [TestMethod]
        public void Map_WhenCalledWithNameAndSupportedImgType_SetsDocImgWithoutAltTextToName()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = _systemUnderTest.Map(individualDocument, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsTrue(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithNonSupportedImageType_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = _systemUnderTest.Map(individualDocument, "doc", "Blood test results alt text");

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithSupportedImageTypeButNoName_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = _systemUnderTest.Map(individualDocument, "jpg", null);

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("alt=\"", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_DocumentContainsNoImg_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            // Act
            //
            var patientDocument = _systemUnderTest.Map(individualDocument, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithSupportedImageTypeAndName_WillNotOverwriteExistingAltText()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithAltText
            };

            // Act
            //
            var patientDocument = _systemUnderTest.Map(individualDocument, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsTrue(patientDocument.Content.Contains("Existing alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapForDownload_WhenMappedPatientDocumentHasErrored_ReturnsNullFileContentResult()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = null
            };

            // Act
            //
            var fileContentResult = _systemUnderTest.MapForDownload(individualDocument, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsNull(fileContentResult);
        }

        [TestMethod]
        public void MapForDownload_WhenNoErrorsAreReturnedAndTypeIsText_ReturnsFileContentResultWithConvertedTextDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            var expectedFileContents = _fixture.Create<byte[]>();
            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToText(DocWithoutImgContent))
                .Returns(expectedFileContents)
                .Verifiable();

            // Act
            //
            var fileContentResult = _systemUnderTest.MapForDownload(individualDocument, "txt", "Blood test results");

            // Assert
            //
            _mockEmisDocumentDownloadConverter.Verify();
            AssertFileContentResult(expectedFileContents, "text/plain", "Blood test results.txt", fileContentResult);
        }

        [TestMethod]
        public void MapForDownload_WhenNoErrorsAreReturnedAndTypeIsAnImage_ReturnsFileContentResultWithConvertedTextDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            var expectedFileContents = _fixture.Create<byte[]>();
            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToImage(DocWithoutImgContent))
                .Returns(expectedFileContents)
                .Verifiable();

            // Act
            //
            var fileContentResult = _systemUnderTest.MapForDownload(individualDocument, "jpg", "Blood test results");

            // Assert
            //
            _mockEmisDocumentDownloadConverter.Verify();
            AssertFileContentResult(expectedFileContents, "image/jpg", "Blood test results.jpg", fileContentResult);
        }

        [TestMethod]
        public void MapForDownload_WhenNoErrorsAreReturnedAndTypeIsDoc_ReturnsFileContentResultWithConvertedWordDocument()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            var expectedFileContents = _fixture.Create<byte[]>();
            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToWordDocument(DocWithoutImgContent))
                .Returns(expectedFileContents)
                .Verifiable();

            // Act
            //
            var fileContentResult = _systemUnderTest.MapForDownload(individualDocument, "doc", "Blood test results");

            // Assert
            //
            _mockEmisDocumentDownloadConverter.Verify();
            AssertFileContentResult(expectedFileContents, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Blood test results.doc", fileContentResult);
        }

        [TestMethod]
        public void MapForDownload_WhenNoErrorsAreReturnedAndTypeIsPdf_ReturnsFileContentResultWithConvertedPdf()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            var expectedFileContents = _fixture.Create<byte[]>();
            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToPdf(DocWithoutImgContent))
                .Returns(expectedFileContents)
                .Verifiable();

            // Act
            //
            var fileContentResult = _systemUnderTest.MapForDownload(individualDocument, "pdf", "Blood test results");

            // Assert
            //
            _mockEmisDocumentDownloadConverter.Verify();
            AssertFileContentResult(expectedFileContents, "application/pdf", "Blood test results.pdf", fileContentResult);
        }

        [TestMethod]
        public void MapForDownload_ConversionReturnsNull_ReturnsNullFileContentResult()
        {
            // Arrange
            //
            var individualDocument = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            byte[] nullData = null;
            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToText(DocWithoutImgContent))
                .Returns(nullData)
                .Verifiable();

            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToImage(DocWithoutImgContent))
                .Returns(nullData)
                .Verifiable();

            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToWordDocument(DocWithoutImgContent))
                .Returns(nullData)
                .Verifiable();

            _mockEmisDocumentDownloadConverter
                .Setup(c => c.ConvertToPdf(DocWithoutImgContent))
                .Returns(nullData)
                .Verifiable();

            // Act
            //
            var textFileContentResult = _systemUnderTest.MapForDownload(individualDocument, "txt", "Blood test results");
            var imageFileContentResult = _systemUnderTest.MapForDownload(individualDocument, "jpg", "Blood test results");
            var wordDocumentFileContentResult = _systemUnderTest.MapForDownload(individualDocument, "doc", "Blood test results");
            var pdfFileContentResult = _systemUnderTest.MapForDownload(individualDocument, "pdf", "Blood test results");

            // Assert
            //
            _mockEmisDocumentDownloadConverter.Verify();
            Assert.IsNull(textFileContentResult);
            Assert.IsNull(imageFileContentResult);
            Assert.IsNull(wordDocumentFileContentResult);
            Assert.IsNull(pdfFileContentResult);
        }

        private static void AssertFileContentResult(byte[] expectedFileContents, string mimeType, string fileName, FileContentResult fileContentResult)
        {
            Assert.AreEqual(expectedFileContents, fileContentResult.FileContents);
            Assert.AreEqual(mimeType, fileContentResult.ContentType);
            Assert.AreEqual(fileName, fileContentResult.FileDownloadName);
        }
    }
}