using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.HttpClients
{
    [TestClass]
    public class ServiceDefinitionIsValidQueryTests
    {
         [TestMethod]
         public async Task ServiceDefinitionIsValidQuery_ProxiesRequestToProvider()
         {
             // Arrange
             //
             var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
             
             handlerMock
                 .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                     "SendAsync",
                     ItExpr.IsAny<HttpRequestMessage>(),
                     ItExpr.IsAny<CancellationToken>())
                 .ReturnsAsync(new HttpResponseMessage
                 {
                     StatusCode = HttpStatusCode.OK,
                     Content = new StringContent("{\"test\": \"value\"}")
                 })
                 .Verifiable();
             
             var mockLogger = new Mock<ILogger<IServiceDefinitionIsValidQuery>>();
             var mockHttpClientFactory = new Mock<IHttpClientFactory>();
             const string Provider = "ProviderKey";
             const string RequestBody = "{\"request\": \"body\"}";

             var httpClient = new HttpClient(handlerMock.Object)
             {
                BaseAddress = new Uri("http://test.com/")
             };
             
             mockHttpClientFactory.Setup(a => a.CreateClient(Provider))
                 .Returns(httpClient);

             var sut = new ServiceDefinitionIsValidQuery(mockLogger.Object,
                 mockHttpClientFactory.Object);

             // Act
             //
             var result = await sut.ServiceDefinitionIsValid(Provider, RequestBody);

             // Assert
             //
             result.Should().NotBeNull();

             handlerMock
                 .Protected()
                 .Verify(
                     "SendAsync",
                     Times.Once(),
                     ItExpr.Is<HttpRequestMessage>(req => 
                         req.Properties.Count == 0
                         && req.Headers.Contains(Constants.OnlineConsultationConstants.ProviderIdentifierHeader)),
                     ItExpr.IsAny<CancellationToken>()
                 );
         }
    }
}