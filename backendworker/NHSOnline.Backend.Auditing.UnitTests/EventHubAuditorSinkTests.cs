using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Auditing.UnitTests
{
    [TestClass]
    public class EventHubAuditorSinkTests
    {
        private Mock<ILogger<EventHubAuditorSink>> _mockLogger;
        private Mock<EventHubProducerClient> _mockEventHubProducerClient;
        private EventHubAuditorSink _eventHubAuditorSink;
        private AuditRecord _auditRecord;
        private EventDataBatch _eventDataBatch;

        [TestInitialize]
        public void TestInitialize()
        {
            const string eventHubConnectionString = "Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=example-sender;SharedAccessKey=fake;EntityPath=auditing-events";

            _mockLogger = new Mock<ILogger<EventHubAuditorSink>>();
            _mockEventHubProducerClient = new Mock<EventHubProducerClient>(eventHubConnectionString);
            _eventHubAuditorSink = new EventHubAuditorSink(_mockLogger.Object, _mockEventHubProducerClient.Object);
        }

        [TestMethod]
        public async Task PreAuditOperation_Created_NoExceptionThrown()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);
            SetupEventDataBatch();

            _mockEventHubProducerClient.Setup(x => x.CreateBatchAsync(It.IsAny<CreateBatchOptions>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EventDataBatch>(_eventDataBatch));

            _mockEventHubProducerClient.Setup(x => x.SendAsync(It.IsAny<EventDataBatch>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _eventHubAuditorSink.WritePreOperationAudit(_auditRecord);
        }

        [TestMethod, ExpectedException(typeof(PreOperationAuditException))]
        public async Task PreAuditOperation_WithEventHubError_ExceptionThrown()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);
            SetupEventDataBatch();

            _mockEventHubProducerClient.Setup(x => x.CreateBatchAsync(It.IsAny<CreateBatchOptions>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            await _eventHubAuditorSink.WritePreOperationAudit(_auditRecord);
        }

        [TestMethod]
        public async Task PostAuditOperation_Created_NoExceptionThrown()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);
            SetupEventDataBatch();

            _mockEventHubProducerClient.Setup(x => x.CreateBatchAsync(It.IsAny<CreateBatchOptions>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EventDataBatch>(_eventDataBatch));

            _mockEventHubProducerClient.Setup(x => x.SendAsync(It.IsAny<EventDataBatch>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _eventHubAuditorSink.WritePostOperationAudit(_auditRecord);
        }

        [TestMethod]
        public async Task PostAuditOperation_WithEventHubError_NoExceptionThrown()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);
            SetupEventDataBatch();

            _mockEventHubProducerClient.Setup(x => x.CreateBatchAsync(It.IsAny<CreateBatchOptions>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            await _eventHubAuditorSink.WritePostOperationAudit(_auditRecord);
        }

        private void SetupAuditRecord(string operation)
        {
            _auditRecord = new AuditRecord(
                DateTime.Now,
                "nhsLoginSubject",
                "nhsNumber",
                false,
                Support.Supplier.Emis,
                operation,
                "details",
                new VersionTag("api", "web", "native"),
                "test",
                null,
                "SessionId",
                "P9",
                "odsCode",
                "referrer",
                "providerId",
                "providerName",
                "jumpOffId");
        }

        private void SetupEventDataBatch()
        {
            const int reportedBatchSize = 1;
            var batchOptions = new CreateBatchOptions { PartitionKey = _auditRecord.NhsLoginSubject };
            _eventDataBatch = EventHubsModelFactory.EventDataBatch(reportedBatchSize, new List<EventData>(), batchOptions);
        }
    }
}
