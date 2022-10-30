using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using Wkhtmltopdf.NetCore;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDocumentMapperTests
    {
        private TppDocumentMapper _mapper;
        private Mock<ILogger<ITppDocumentMapper>> _mockLogger;
        private Mock<IGeneratePdf> _mockPdfGenerator;
        private const string AltDescription = "This file has no description, please contact your GP surgery for more information";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<ITppDocumentMapper>>();
            _mockPdfGenerator = new Mock<IGeneratePdf>();

            _mapper = new TppDocumentMapper(_mockLogger.Object, _mockPdfGenerator.Object);
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
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = null,
                Type = "invalid",
                HasErrored = false,
                IsViewable = false,
                IsDownloadable = false,
                PageCount = 1
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
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = $"<div id=\"document_0\" class=\"documentContainer nhsuk-u-margin-top-5\"><img alt=\"{AltDescription}\" src=\"data:{mimeType};base64,test\"/></div>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false,
                IsViewable = true,
                IsDownloadable = true,
                PageCount = 1
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
        public void Map_ViewableTypeWithMultiplePages_ReturnsSuccessfulResult(string fileType, string mimeType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "page1"
                        },
                        new BinaryDataPage
                        {
                            BinaryData = "page2"
                        },
                        new BinaryDataPage
                        {
                            BinaryData = "page3"
                        }
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = $"<div id=\"document_0\" class=\"documentContainer nhsuk-u-margin-top-5\"><img alt=\"{AltDescription}\" src=\"data:{mimeType};base64,page1\"/></div><div id=\"document_1\" class=\"documentContainer nhsuk-u-margin-top-5\"><img alt=\"{AltDescription}\" src=\"data:{mimeType};base64,page2\"/></div><div id=\"document_2\" class=\"documentContainer nhsuk-u-margin-top-5\"><img alt=\"{AltDescription}\" src=\"data:{mimeType};base64,page3\"/></div>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false,
                IsViewable = true,
                IsDownloadable = true,
                PageCount = 3
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
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
                    }
                }
            };

            var expectedPatientDocument = new PatientDocument
            {
                Content = null,
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false,
                IsViewable = false,
                IsDownloadable = true,
                PageCount = 1
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
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
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
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = content
                        }
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
        [DataRow("JPG", "image/jpg", "jpg")]
        [DataRow("BMP", "image/bmp", "bmp")]
        [DataRow("DIB", "image/dib", "dib")]
        [DataRow("GIF", "image/gif", "gif")]
        [DataRow("JPEG", "image/jpg", "jpeg")]
        [DataRow("JPE", "image/jpg", "jpe")]
        [DataRow("JFIF", "image/jpg", "jpg")]
        [DataRow("PNG", "image/png", "png")]
        [DataRow("TIF", "image/tiff", "tif")]
        [DataRow("TIFF", "image/tiff", "tiff")]
        [DataRow("DOC", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "doc")]
        [DataRow("DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")]
        [DataRow("DOCM", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docm")]
        [DataRow("DOT", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "dot")]
        [DataRow("PDF", "application/pdf", "pdf")]
        [DataRow("TXT", "text/plain", "txt")]
        [DataRow("RTF", "application/rtf", "rtf")]
        public void MapForDownload_WithValidRequestBinaryDataReply_ReturnsSuccessfulFileContentResult(
            string fileType, string mimeType, string mappedType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
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

        [TestMethod]
        [DataRow("jpg")]
        [DataRow("bmp")]
        [DataRow("dib")]
        [DataRow("gif")]
        [DataRow("jpeg")]
        [DataRow("jpe")]
        [DataRow("jfif")]
        [DataRow("png")]
        [DataRow("JPG")]
        [DataRow("BMP")]
        [DataRow("DIB")]
        [DataRow("GIF")]
        [DataRow("JPEG")]
        [DataRow("JPE")]
        [DataRow("JFIF")]
        [DataRow("PNG")]
        public void MapForDownload_WithMultiPageViewableDocument_ReturnsPDFContentResult(
            string fileType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        },
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        }
                    }
                }
            };

            _mockPdfGenerator.Setup(x => x.GetPDF(It.IsAny<string>()))
                .Returns(new byte[] { 181, 235, 45 });

            var expectedFileContentResult = new FileContentResult(new byte[] { 181, 235, 45 }, "application/pdf")
            {
                FileDownloadName = $"documentName.pdf"
            };

            // Act
            var fileContentResult = _mapper.MapForDownload(binaryRequestResponse, "documentName");

            // Assert
            fileContentResult.Should().BeEquivalentTo(expectedFileContentResult);
        }

        [TestMethod]
        [DataRow("doc", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "doc")]
        [DataRow("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")]
        [DataRow("docm", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docm")]
        [DataRow("dot", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "dot")]
        [DataRow("pdf", "application/pdf", "pdf")]
        [DataRow("txt", "text/plain", "txt")]
        [DataRow("rtf", "application/rtf", "rtf")]
        [DataRow("DOC", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "doc")]
        [DataRow("DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx")]
        [DataRow("DOCM", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docm")]
        [DataRow("DOT", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "dot")]
        [DataRow("PDF", "application/pdf", "pdf")]
        [DataRow("TXT", "text/plain", "txt")]
        [DataRow("RTF", "application/rtf", "rtf")]
        public void MapForDownload_WithMultiPageNonViewableDocument_ReturnsFirstPageOnly(
            string fileType, string mimeType, string mappedType)
        {
            // Arrange
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = fileType,
                    BinaryDataPages = new List<BinaryDataPage>
                    {
                        new BinaryDataPage
                        {
                            BinaryData = "test"
                        },
                        new BinaryDataPage
                        {
                            BinaryData = "test2"
                        }
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