using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Auditing.UnitTests
{
    [TestClass]
    public  class DbAuditorSinkTests
    {
        private Mock<ILogger<DbAuditorSink>> _mockLogger;
        private Mock<IRepository<AuditRecord>> _mockRepository;

        private DbAuditorSink _dbAuditorSink;
        private AuditRecord _auditRecord;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<DbAuditorSink>>();
            _mockRepository = new Mock<IRepository<AuditRecord>>();

            _dbAuditorSink = new DbAuditorSink(
                _mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod, ExpectedException(typeof(PreOperationAuditException))]
        public async Task ExceptionThrown_WhenPreAuditOperation_And_RepositoryError()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);

            _mockRepository
                .Setup(repo => repo.Create(_auditRecord, nameof(AuditRecord)))
                .ReturnsAsync(new RepositoryCreateResult<AuditRecord>.RepositoryError());

            await _dbAuditorSink.WritePreOperationAudit(_auditRecord);
        }

        [TestMethod, ExpectedException(typeof(PreOperationAuditException))]
        public async Task ExceptionThrown_WhenPreAuditOperation_And_RepositoryErrorWithException()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);

            _mockRepository
                .Setup(repo => repo.Create(_auditRecord, nameof(AuditRecord)))
                .ReturnsAsync(new RepositoryCreateResult<AuditRecord>.RepositoryError(new MongoException("error")));

            await _dbAuditorSink.WritePreOperationAudit(_auditRecord);
        }

        [TestMethod]
        public async Task NoException_WhenPreAuditOperation_And_CreatedResult()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);

            _mockRepository
                .Setup(repo => repo.Create(_auditRecord, nameof(AuditRecord)))
                .ReturnsAsync(new RepositoryCreateResult<AuditRecord>.Created(_auditRecord));

            await _dbAuditorSink.WritePreOperationAudit(_auditRecord);
        }

        [TestMethod]
        public async Task NoException_WhenPostAuditOperation_And_Created()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsRequest);

            _mockRepository
                .Setup(repo => repo.Create(_auditRecord, nameof(AuditRecord)))
                .ReturnsAsync(new RepositoryCreateResult<AuditRecord>.Created(_auditRecord));

            await _dbAuditorSink.WritePostOperationAudit(_auditRecord);
        }

        [TestMethod]
        public async Task NoException_WhenPostAuditOperation_And_RepositoryError()
        {
            SetupAuditRecord(AuditingOperations.GetLinkedAccountsResponse);

            _mockRepository
                .Setup(repo => repo.Create(_auditRecord, nameof(AuditRecord)))
                .ReturnsAsync(new RepositoryCreateResult<AuditRecord>.RepositoryError());

            await _dbAuditorSink.WritePostOperationAudit(_auditRecord);
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
                new VersionTag("api","web","native"));
        }
    }
}
