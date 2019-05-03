using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public class HttpRequestIdentityTests
    {
        private IFixture _fixture;
        private HttpRequestIdentity _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void ToString_DefaultInstance_HappyPath()
        {
            var provider = _fixture.Create<string>();
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            
            _systemUnderTest = new HttpRequestIdentity(provider,requestMessage);

            var result = _systemUnderTest.ToString();
            
            result.Should().ContainAll(
                $"Provider={provider}", 
                $"UpStreamMethod={requestMessage.Method}", 
                $"UpStreamUrl={requestMessage.RequestUri}");
            
            result.Should().NotContainAny("UpStreamIdentifier", "CorrelationIdentifier");
        }
        
        [TestMethod]
        public void ToString_WithIdentifier_HappyPath()
        {
            var provider = _fixture.Create<string>();
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var identifier = _fixture.Create<string>();
            
            _systemUnderTest = new HttpRequestIdentity(provider,requestMessage)
                .SetUpStreamIdentifier(identifier);
            
            var result = _systemUnderTest.ToString();
            
            result.Should().ContainAll(
                $"Provider={provider}", 
                $"UpStreamMethod={requestMessage.Method}", 
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"UpStreamIdentifier={identifier}");
            
            result.Should().NotContain("CorrelationIdentifier");
        }
        
        [TestMethod]
        public void ToString_WithCorrelationIdentifier_HappyPath()
        {
            var provider = _fixture.Create<string>();
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var correlationIdentifier = _fixture.Create<string>();
            
            _systemUnderTest = new HttpRequestIdentity(provider,requestMessage)
                .SetCorrelationIdentifier(correlationIdentifier);
            
            var result = _systemUnderTest.ToString();
            
            result.Should().ContainAll(
                $"Provider={provider}", 
                $"UpStreamMethod={requestMessage.Method}", 
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"CorrelationIdentifier={correlationIdentifier}");
            
            result.Should().NotContain("UpStreamIdentifier");
        }
        
        [TestMethod]
        public void ToString_WithCorrelationIdentifierAndIdentifier_HappyPath()
        {
            var provider = _fixture.Create<string>();
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var correlationIdentifier = _fixture.Create<string>();
            var identifier = _fixture.Create<string>();
            
            _systemUnderTest = new HttpRequestIdentity(provider,requestMessage)
                .SetCorrelationIdentifier(correlationIdentifier)
                .SetUpStreamIdentifier(identifier);
            
            var result = _systemUnderTest.ToString();
            
            result.Should().ContainAll(
                $"Provider={provider}", 
                $"UpStreamMethod={requestMessage.Method}", 
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"CorrelationIdentifier={correlationIdentifier}",
                $"UpStreamIdentifier={identifier}");
        }
    }
}