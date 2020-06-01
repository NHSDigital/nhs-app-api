using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsRepositoryTests
    {
        private IFixture _fixture;
        private ITermsAndConditionsRepository _systemUnderTest;
        private Mock<IMongoCollection<TermsAndConditionsRecord>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<TermsAndConditionsRecord>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<TermsAndConditionsRecord>(It.IsAny<string>(), null))
                .Returns(_mongoCollectionMock.Object);

            var mockMongoClient = _fixture.Freeze<Mock<IApiMongoClient<IMongoConfiguration>>>();
            mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            _systemUnderTest = _fixture.Create<TermsAndConditionsRepository>();
        }

        [TestMethod]
        public void Create_WhenRecordIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Create(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("record", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Create_WithRecord_AddsToCollection()
        {
            // Arrange
            var record = _fixture.Create<TermsAndConditionsRecord>();
            _mongoCollectionMock.Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Create(record);

            // Assert
            _mongoCollectionMock.VerifyAll();
        }

        [TestMethod]
        public async Task Find_WhenNhsLoginIdRecordExists_ReturnsRecord()
        {
            // Arrange
            var record = _fixture.Create<TermsAndConditionsRecord>();
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[] { record });

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<TermsAndConditionsRecord>>(),
                    It.IsAny<FindOptions<TermsAndConditionsRecord, TermsAndConditionsRecord>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(record.NhsLoginId);

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().Be(record);
        }


        [TestMethod]
        public async Task Find_WhenNhsLoginIdRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<TermsAndConditionsRecord>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<TermsAndConditionsRecord>>(),
                    It.IsAny<FindOptions<TermsAndConditionsRecord, TermsAndConditionsRecord>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeNull();
        }

        [TestMethod]
        public void Find_WithNullNhsLoginId_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(null);

            // Assert
            act.Should().Throw<AggregateException>();
        }

        [TestMethod]
        public void Update_WhenRecordIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Update(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("record", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Update_WithRecord_UpdatesRecord()
        {
            // Arrange
            var updatedRecord = _fixture.Create<TermsAndConditionsRecord>();

            // Act
            await _systemUnderTest.Update(updatedRecord);

            // Assert
            _mongoCollectionMock.Verify(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<TermsAndConditionsRecord>>(),
                updatedRecord,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()
            ));
            _mongoCollectionMock.VerifyNoOtherCalls();
        }
    }
}
