using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsRepositoryTests
    {
        private ITermsAndConditionsRepository _systemUnderTest;
        private Mock<IRepository<TermsAndConditionsRecord>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<TermsAndConditionsRecord>>();

            _systemUnderTest = new TermsAndConditionsRepository(new Mock<ILogger<TermsAndConditionsRepository>>().Object,
                _mockRepository.Object);
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
            var record = new TermsAndConditionsRecord();
            _mockRepository.Setup(x =>
                    x.Create(It.IsAny<TermsAndConditionsRecord>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryCreateResult<TermsAndConditionsRecord>.Created(record));

            // Act
            var result = await _systemUnderTest.Create(record);

            // Assert
            result.Should().BeOfType<RepositoryCreateResult<TermsAndConditionsRecord>.Created>()
                .Subject.Record.Should().BeEquivalentTo( record );
        }

        [TestMethod]
        public async Task Find_WhenNhsLoginIdRecordExists_ReturnsRecord()
        {
            // Arrange
            var record = new TermsAndConditionsRecord();

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<TermsAndConditionsRecord, bool>>>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new RepositoryFindResult<TermsAndConditionsRecord>.Found(new []{record}));

            // Act
            var result = await _systemUnderTest.Find("nhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<TermsAndConditionsRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new []{record});
        }


        [TestMethod]
        public async Task Find_WhenNhsLoginIdRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<TermsAndConditionsRecord, bool>>>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new RepositoryFindResult<TermsAndConditionsRecord>.NotFound());

            // Act
            var result = await _systemUnderTest.Find("nhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<TermsAndConditionsRecord>.NotFound>();
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
        public void Update_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Update(null, new UpdateRecordBuilder<TermsAndConditionsRecord>());

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("nhsLoginId", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Update_WithRecord_UpdatesRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Update(It.IsAny<Expression<Func<TermsAndConditionsRecord, bool>>>(), It.IsAny<UpdateRecordBuilder<TermsAndConditionsRecord>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryUpdateResult<TermsAndConditionsRecord>.Updated());
            var updatedRecord = new TermsAndConditionsRecord();

            // Act
            var result = await _systemUnderTest.Update("nhsLoginId", new UpdateRecordBuilder<TermsAndConditionsRecord>());

            // Assert
            result.Should().BeOfType<RepositoryUpdateResult<TermsAndConditionsRecord>.Updated>();
        }
    }
}
