using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDocumentMapperTests
    {
        private TppDocumentMapper _mapper;
        private Mock<ILogger<ITppDocumentMapper>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<ITppDocumentMapper>>();

            _mapper = new TppDocumentMapper(_mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Map_WhenRequestBinaryDataReplyIsNull_ThrowsArgumentNullException()
        {
            _mapper.Map(null);
        }

        [TestMethod]
        public void Map_WithInvalidType_ReturnsNonViewableAndNonDownloadableResult()
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "invalid",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test"
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = null,
                Type = "invalid",
                HasErrored = false,
                IsViewable = false,
                IsDownloadable = false
            };

            // Act
            var patientDocument = _mapper.Map(binaryRequestResponse);

            // Assert
            patientDocument.Should().NotBeNull();
            patientDocument.Should().BeEquivalentTo(expectedPatientDocument);
        }

        [DataTestMethod]
        [DataRow("jpg", "image/jpg")]
        [DataRow("bmp", "image/bmp")]
        [DataRow("dib", "image/dib")]
        [DataRow("gif", "image/gif")]
        [DataRow("jpeg", "image/jpg")]
        [DataRow("jpe", "image/jpg")]
        [DataRow("jfif", "image/jpg")]
        [DataRow("png", "image/png")]
        public void Map_ViewableType_ReturnsSuccessfulResult(string fileType, string mimeType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test"
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = $"<img src=\"data:{mimeType};base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false,
                IsViewable = true,
                IsDownloadable = true
            };

            // Act
            var patientDocument = _mapper.Map(binaryRequestResponse);

            // Assert
            patientDocument.Should().NotBeNull();
            patientDocument.Should().BeEquivalentTo(expectedPatientDocument);
        }

        [DataTestMethod]
        [DataRow("tif")]
        [DataRow("tiff")]
        [DataRow("doc")]
        [DataRow("docx")]
        [DataRow("docm")]
        [DataRow("dot")]
        [DataRow("pdf")]
        [DataRow("txt")]
        [DataRow("rtf")]
        public void Map_WithDownloadableOnlyType_ReturnsSuccessfulResultWithNullContentAndViewableFalse(string fileType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test"
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = null,
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false,
                IsViewable = false,
                IsDownloadable = true
            };

            // Act
            var patientDocument = _mapper.Map(binaryRequestResponse);

            // Assert
            patientDocument.Should().NotBeNull();
            patientDocument.Should().BeEquivalentTo(expectedPatientDocument);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapForDownload_WithNullRequestBinaryDataReply_ThrowsArgumentNullException()
        {
            _mapper.MapForDownload(null, "documentName");
        }

        [TestMethod]
        public void MapForDownload_WithInvalidType_ReturnsNullResult()
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "invalid",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test"
                    }
                }
            };

            // Act
            var patientDocument = _mapper.MapForDownload(binaryRequestResponse, "documentName");

            // Assert
            patientDocument.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void MapForDownload_WhenRequestBinaryDataIsNullOrEmpty_ReturnsNull(string content)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpg",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = content
                    }
                }
            };

            // Act
            var fileContentResult = _mapper.MapForDownload(binaryRequestResponse, "documentName");

            // Assert
            fileContentResult.Should().BeNull();
        }

        [TestMethod]
        [DataRow("jpg", "image/jpg", "jpg")]
        [DataRow("bmp", "image/bmp", "bmp")]
        [DataRow("dib", "image/dib", "dib")]
        [DataRow("gif", "image/gif", "gif")]
        [DataRow("jpeg", "image/jpg", "jpeg")]
        [DataRow("jpe", "image/jpg", "jpe")]
        [DataRow("jfif", "image/jpg", "jpg")]
        [DataRow("png", "image/png", "png")]
        [DataRow("tif", "image/tiff", "tif")]
        [DataRow("tiff", "image/tiff", "tiff")]
        [DataRow("doc", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "doc")]
        [DataRow("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")]
        [DataRow("docm", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docm")]
        [DataRow("dot", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "dot")]
        [DataRow("pdf", "application/pdf", "pdf")]
        [DataRow("txt", "text/plain", "txt")]
        [DataRow("rtf", "application/rtf", "rtf")]
        public void MapForDownload_WithValidRequestBinaryDataReply_ReturnsSuccessfulFileContentResult(
            string fileType, string mimeType, string mappedType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test"
                    }
                }
            };
            var expectedFileContentResult = new FileContentResult(new byte[] { 181, 235, 45 }, mimeType)
            {
                FileDownloadName = $"documentName.{mappedType}"
            };

            // Act
            var fileContentResult = _mapper.MapForDownload(binaryRequestResponse, "documentName");

            // Assert
            fileContentResult.Should().BeEquivalentTo(expectedFileContentResult);
        }
    }
}