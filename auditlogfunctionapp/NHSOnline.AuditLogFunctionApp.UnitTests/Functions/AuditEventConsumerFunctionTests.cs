using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.AuditLogFunctionApp.Functions;
using NHSOnline.AuditLogFunctionApp.Model;

namespace NHSOnline.AuditLogFunctionApp.UnitTests.Functions
{
    [TestClass]
    public class AuditEventConsumerFunctionTests
    {
        private AuditRecord[] _auditRecords;
        private Mock<IDocumentClient> _mockCosmosClient;
        private AuditEventConsumerFunction _systemUnderTest;
        private Mock<IConfigurationSection> _mockConfSection;

         [TestInitialize]
        public void TestInitialize()
        {
            _mockConfSection = new Mock<IConfigurationSection>();
            _mockConfSection.SetupGet(m =>
                m[It.Is<string>(s => s == "AuditCosmosSQLDbName")]
            ).Returns("db");
            _mockConfSection.SetupGet(m =>
                m[It.Is<string>(s => s == "AuditCosmosSQLDbContainer")]
            ).Returns("container");

            _systemUnderTest = new AuditEventConsumerFunction(
                _mockConfSection.Object,
                Mock.Of<ILogger<AuditEventConsumerFunction>>()
            );
            _mockCosmosClient = new Mock<IDocumentClient>();
        }

        [TestMethod]
        public async Task AuditEventConsumerFunction_NoAuditRecords_DbUpsertNotInvoked()
        {
            // Act
            await _systemUnderTest.AuditEventConsumerTrigger(new AuditRecord[0], _mockCosmosClient.Object);

            // Assert
            _mockCosmosClient.Verify(
                m => m.UpsertDocumentAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<AuditRecord>(),
                    It.IsAny<RequestOptions>(),
                    false,
                    default
                ),
                Times.Never
            );
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]
        [DataRow(10)]
        public async Task AuditEventConsumerFunction_ValidAuditRecords_DbUpsertCompleted(int auditRecordCount)
        {
            // Arrange
            SetupAuditRecords(auditRecordCount);

            // Act
            await _systemUnderTest.AuditEventConsumerTrigger(_auditRecords, _mockCosmosClient.Object);

            // Assert
            _mockCosmosClient.Verify(
                m => m.UpsertDocumentAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<AuditRecord>(),
                    It.IsAny<RequestOptions>(),
                    false,
                    default
                ),
                Times.Exactly(auditRecordCount));
        }

        private void SetupAuditRecords(int auditRecordCount)
        {
            var auditRecords = new List<AuditRecord>();

            for (var i = 0; i < auditRecordCount; i++)
                auditRecords.Add(new AuditRecord(
                    DateTime.Now.AddHours(i),
                    "nhsLoginSubject",
                    "nhsNumber",
                    false,
                    "Emis",
                    "OnlineConsultations_Submitted",
                    "details",
                    new VersionTag("api", "web", "native"),
                    Guid.NewGuid().ToString()));

            _auditRecords = auditRecords.ToArray();
        }
    }
}
