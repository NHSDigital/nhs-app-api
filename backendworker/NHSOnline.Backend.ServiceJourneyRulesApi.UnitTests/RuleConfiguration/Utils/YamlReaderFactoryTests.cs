using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class YamlReaderFactoryTests
    {
        private YamlReaderFactory _yamlReaderFactory;
        
        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _yamlReaderFactory = fixture.Create<YamlReaderFactory>();
        }

        [TestMethod]
        public void GetReader_OnFirstCall_ReturnNewReader()
        {
            // Act
            var reader = _yamlReaderFactory.GetReader<object>("c:/foo.yaml", new FileData(null, null));
            
            // Assert
            reader.Should().NotBeNull();
        }

        [TestMethod]
        public void GetReader_WhenCached_ReturnExistingReader()
        {
            // Act
            var reader = _yamlReaderFactory.GetReader<object>("c:/foo.yaml", new FileData(null, null));
            var cachedReader = _yamlReaderFactory.GetReader<object>("c:/foo.yaml", new FileData(null, null));
            
            // Assert
            reader.Should().NotBeNull();
            cachedReader.Should().BeSameAs(reader);
        }
    }
}