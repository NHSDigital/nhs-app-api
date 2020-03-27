using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public sealed class YamlNodeDeserializerTests : IDisposable
    {
        private IFixture _fixture;
        private YamlNodeDeserializer _yamlNodeDeserializer;

        private Mock<IDeserializer> _mockDeserializer;
        private Mock<ITextReaderBuilder<TextReader>> _mockStringReaderBuilder;
        private Mock<IParserBuilder<IParser>> _mockParserBuilder;
        private Mock<ILogger<YamlNodeDeserializer>> _mockLogger;

        private Mock<IParser> _mockReader;
        private Mock<IParser> _mockNodeParser;
        private StringReader _stringReader;

        private string _baseIncludePath;
        private string _tag;
        private const string ExistingFileName = "publicHealthNotification";
        private const string NonExistingFileName = "nonExistingPublicHealthNotification";
        private readonly List<Type> _supportedTypes = new List<Type> { typeof(PublicHealthNotification) };

        private object _expectedDeserializedResponse;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockDeserializer = _fixture.Create<Mock<IDeserializer>>();
            _mockStringReaderBuilder = _fixture.Create<Mock<ITextReaderBuilder<TextReader>>>();
            _mockParserBuilder = _fixture.Create<Mock<IParserBuilder<IParser>>>();
            _mockLogger = _fixture.Create<Mock<ILogger<YamlNodeDeserializer>>>();

            _mockNodeParser = _fixture.Create<Mock<IParser>>();
            _mockReader = _fixture.Create<Mock<IParser>>();
            _mockReader.Setup(r => r.MoveNext()).Returns(true);

            _tag = _fixture.Create<string>();
            _baseIncludePath = _fixture.Create<string>();

            _yamlNodeDeserializer = new YamlNodeDeserializer(
                _mockDeserializer.Object,
                _tag,
                _baseIncludePath,
                _supportedTypes,
                _mockStringReaderBuilder.Object,
                _mockParserBuilder.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_WhenUnsupportedModelSpecified_ThrowsInvalidOperationException()
        {
            SetupMocks<object>();

            Deserialize(out _);

            VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Deserialize_WhenReaderThrowsFileNotFoundException_ThrowsFileNotFoundException()
        {
            SetupMocks<PublicHealthNotification>(useMissingFile: true, throwsFileNotFound: true);

            Deserialize(out _);

            VerifyAll();
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void Deserialize_WhenNextParsingEventIsNullOrHasInvalidTag_ReturnsNullValueAndFalse(
            bool parsingEventIsNull,
            bool hasValidTag)
        {
            SetupMocks<PublicHealthNotification>(parsingEventIsNull, hasValidTag);

            var deserialized = Deserialize(out var value);
            
            Assert.IsNull(value);
            Assert.IsFalse(deserialized);
        }

        [TestMethod]
        public void Deserialize_HappyPath_ReturnsDeserializedValueAndTrue()
        {
            SetupMocks<PublicHealthNotification>();
            
            var deserialized = Deserialize(out var value);
            
            Assert.IsNotNull(value);
            Assert.AreSame(_expectedDeserializedResponse, value);
            Assert.IsTrue(deserialized);

            VerifyAll();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _stringReader?.Dispose();
        }

        private void SetupMocks<T>(
            bool parsingEventIsNull = false,
            bool useValidTag = true,
            bool useMissingFile = false,
            bool throwsFileNotFound = false)
        {
            var modelName = typeof(T).Name;
            var fileName = useMissingFile ? NonExistingFileName : ExistingFileName;
            
            var tag = useValidTag ? _tag : _fixture.Create<string>();
            var value = $"{modelName}s/{fileName}";
            
            var fullFileName = $"{_baseIncludePath}/{value}.yaml";
            
            MockTextReaderBuilder(fullFileName, throwsFileNotFound);
            MockParserBuilder();
            MockDeserializer<T>();
            CreateReader(parsingEventIsNull, tag, value);
        }

        private void MockTextReaderBuilder(string fullFileName, bool throwsFileNotFound)
        {
            _stringReader = new StringReader(fullFileName);

            var setup = _mockStringReaderBuilder
                .Setup(s => s
                    .GetReader(It.Is<string>(
                        text => text.Equals(fullFileName, StringComparison.Ordinal))));

            if (throwsFileNotFound)
            {
                setup.Throws<FileNotFoundException>()
                    .Verifiable();
                return;
            }
            
            setup.Returns(_stringReader)
                .Verifiable();
        }

        private void MockParserBuilder()
        {
            _mockParserBuilder
                .Setup(p => p
                    .GetParser(
                        It.Is<TextReader>(r => r == _stringReader)))
                .Returns(_mockNodeParser.Object)
                .Verifiable();
        }

        private void MockDeserializer<T>()
        {
            _expectedDeserializedResponse = _fixture.Create<T>();

            _mockDeserializer
                .Setup(d => d
                    .Deserialize(
                        It.Is<IParser>(p => p == _mockNodeParser.Object),
                        It.Is<Type>(t => t == typeof(T))))
                .Returns(_expectedDeserializedResponse)
                .Verifiable();
        }

        private void CreateReader(bool parsingEventIsNull, string tag, string value)
        {
            var parsingEvent = parsingEventIsNull
                ? null
                : new Scalar(tag, value);

            _mockReader
                .Setup(r => r.Current)
                .Returns(parsingEvent);
        }

        private bool Deserialize(out object value)
        {
            return _yamlNodeDeserializer.Deserialize(_mockReader.Object, null, null, out value);
        }

        private void VerifyAll()
        {
            _mockStringReaderBuilder.Verify();
            _mockParserBuilder.Verify();
            _mockDeserializer.Verify();
        }

        public void Dispose()
        {
            TestCleanup();
        }
    }
}