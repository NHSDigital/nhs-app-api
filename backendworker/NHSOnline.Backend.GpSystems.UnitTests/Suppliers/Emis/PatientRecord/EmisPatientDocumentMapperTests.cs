using System;
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
        private Mock<ILogger<EmisPatientDocumentMapper>> _mockLogger;

        private const string DocWithImgWithAltText =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLPJzE1XKC5KtlUqLilNS1NSSMwpsVVyrcgsLsnMSwfxFEpSK0qU7Gz0Ier1wZoBkzfeT0MAAAA=";

        private const string DocWithImgWithoutAltText =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLPJzE1XKC5KtlUqLilNS1Oys9GHiOuDFQEAVGaspysAAAA=";

        private const string DocWithoutImg =
            "H4sIAAAAAAAAE7PJKMnNsbNJyk+ptLMpsAvOz01VyC/JSC1SSM7PK0nNK7HRL7Cz0YfI64MVAwCk81DYMwAAAA==";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<EmisPatientDocumentMapper>>();
        }

        [TestMethod]
        public void Map_WhenCalledWithNameAndSupportedImgType_SetsDocImgWithoutAltTextToName()
        {
            // Arrange
            //
            var systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer());
            var documentGetResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = systemUnderTest.Map(documentGetResponse, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsTrue(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithNonSupportedImageType_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer());
            var documentGetResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = systemUnderTest.Map(documentGetResponse, "doc", "Blood test results alt text");

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithSupportedImageTypeButNoName_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer());
            var documentGetResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithoutAltText
            };

            // Act
            //
            var patientDocument = systemUnderTest.Map(documentGetResponse, "jpg", null);

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("alt=\"", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_DocumentContainsNoImg_WillNotAddAltTextToDocument()
        {
            // Arrange
            //
            var systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer());
            var documentGetResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithoutImg
            };

            // Act
            //
            var patientDocument = systemUnderTest.Map(documentGetResponse, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsFalse(patientDocument.Content.Contains("Blood test results alt text", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_WhenCalledWithSupportedImageTypeAndName_WillNotOverwriteExistingAltText()
        {
            // Arrange
            //
            var systemUnderTest = new EmisPatientDocumentMapper(_mockLogger.Object, new HtmlSanitizer());
            var documentGetResponse = new IndividualDocument
            {
                CompressedEncodedDocumentContent = DocWithImgWithAltText
            };

            // Act
            //
            var patientDocument = systemUnderTest.Map(documentGetResponse, "jpg", "Blood test results alt text");

            // Assert
            //
            Assert.IsTrue(patientDocument.Content.Contains("Existing alt text", StringComparison.Ordinal));
        }
    }
}