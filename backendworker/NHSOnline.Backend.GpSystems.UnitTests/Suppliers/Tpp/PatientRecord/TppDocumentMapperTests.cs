using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDocumentMapperTests
    {
        private TppDocumentMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppDocumentMapper();
        }

        [TestMethod]
        public void MapRequestBinaryData_WithJpgPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpg",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/jpg;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithBmpPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "bmp",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/bmp;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithDibPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "dib",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/dib;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithGifPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "gif",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/gif;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithJpegPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpeg",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/jpg;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithJpePopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jpe",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/jpg;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithJfifPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "jfif",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/jpg;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithPngPopulatedValues_ReturnsResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "png",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                Content = "<img src=\"data:image/png;base64,test\"/>",
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }

        [TestMethod]
        public void MapRequestBinaryData_WithInvalidType_ReturnsErrorResult()
        {
            var binaryRequestResponse = new RequestBinaryDataReply
            {
                BinaryData = new BinaryDataElement
                {
                    FileType = "rtf",
                    BinaryDataPage = new BinaryDataPage
                    {
                        BinaryData = "test",
                    }
                }
            };

            var expectedMappedReturn = new PatientDocument
            {
                HasErrored = true
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }
    }
}