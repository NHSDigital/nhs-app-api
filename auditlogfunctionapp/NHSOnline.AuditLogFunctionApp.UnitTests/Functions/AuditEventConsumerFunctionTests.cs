using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
        private Mock<IAsyncCollector<AuditRecord>> _mockCosmosClient;
        private AuditEventConsumerFunction _systemUnderTest;

         [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new AuditEventConsumerFunction(
                Mock.Of<ILogger<AuditEventConsumerFunction>>()
            );
            _mockCosmosClient = new Mock<IAsyncCollector<AuditRecord>>();
        }

        [TestMethod]
        public async Task AuditEventConsumerFunction_NoAuditRecords_DbUpsertNotInvoked()
        {
            // Act
            await _systemUnderTest.AuditEventConsumerTrigger(Array.Empty<AuditRecord>(), _mockCosmosClient.Object);

            // Assert
            _mockCosmosClient.Verify(
                m => m.AddAsync(It.IsAny<AuditRecord>(), default),
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
                m => m.AddAsync(It.IsAny<AuditRecord>(), default),
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
                    Guid.NewGuid().ToString(),
                    "test",
                    "testReferrer"));

            _auditRecords = auditRecords.ToArray();
        }
    }
}
