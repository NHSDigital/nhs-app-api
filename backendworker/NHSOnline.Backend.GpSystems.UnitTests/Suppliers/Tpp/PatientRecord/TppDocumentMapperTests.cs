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
        public void MapRequestPatientRecordReplyTppDcrEventsResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
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
                Type = binaryRequestResponse.BinaryData.FileType,
                HasErrored = false
            };

            // Act
            var tppDcrEvents = _mapper.Map(binaryRequestResponse);

            // Assert
            tppDcrEvents.Should().NotBeNull();
            tppDcrEvents.Should().BeEquivalentTo(expectedMappedReturn);
        }
    }
}