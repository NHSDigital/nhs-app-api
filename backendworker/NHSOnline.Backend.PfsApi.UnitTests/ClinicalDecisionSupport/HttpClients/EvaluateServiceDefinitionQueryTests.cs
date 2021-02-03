 using System;
 using System.Linq;
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
 using NHSOnline.Backend.Support.Settings;

 namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.HttpClients
 {
     [TestClass]
     public sealed class EvaluateServiceDefinitionQueryTests
     {
         [TestMethod]
         public async Task EvaluateServiceDefinitionQuery__ProxiesRequestToProvider()
         {
             // Arrange
             var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
             var olcConfigSettings = new OnlineConsultationsConfigurationSettings(30);

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

             var mockLogger = new Mock<ILogger<EvaluateServiceDefinitionQuery>>();
             var mockHttpClientFactory = new Mock<IHttpClientFactory>();
             const string Provider = "ProviderKey";
             const string ServiceDefinitionId = "SOME_SD";
             const string RequestBody = "{\"request\": \"body\"}";

             var httpClient = new HttpClient(handlerMock.Object)
             {
                BaseAddress = new Uri("http://test.com/")
             };

             mockHttpClientFactory.Setup(a => a.CreateClient(Provider))
                 .Returns(httpClient);

             var sut = new EvaluateServiceDefinitionQuery(mockLogger.Object,
                 mockHttpClientFactory.Object, olcConfigSettings);

             // Act
             var result = await sut.EvaluateServiceDefinition(Provider,
                 ServiceDefinitionId,
                 RequestBody,
                 false,
                 "1");

             // Assert
             result.Should().NotBeNull();

             handlerMock.Protected().Verify(
                 "SendAsync",
                 Times.Once(),
                 ItExpr.Is<HttpRequestMessage>(req =>
                     req.Properties.Count == 1
                     && req.Headers.All(a => !string.Equals(a.Key, Constants.HttpHeaders.JavascriptDisabled, StringComparison.Ordinal))
                 ),
                 ItExpr.IsAny<CancellationToken>()
             );
         }

         [TestMethod]
         public async Task EvaluateServiceDefinitionQuery__Adds_A_JavaScriptDisabledHeader()
         {
             // Arrange
             var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
             var olcConfigSettings = new OnlineConsultationsConfigurationSettings(30);

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

             var mockLogger = new Mock<ILogger<EvaluateServiceDefinitionQuery>>();
             var mockHttpClientFactory = new Mock<IHttpClientFactory>();
             const string Provider = "ProviderKey";
             const string ServiceDefinitionId = "SOME_SD";
             const string RequestBody = "{\"request\": \"body\"}";

             var httpClient = new HttpClient(handlerMock.Object)
             {
                BaseAddress = new Uri("http://test.com/")
             };

             mockHttpClientFactory.Setup(a => a.CreateClient(Provider))
                 .Returns(httpClient);

             var sut = new EvaluateServiceDefinitionQuery(mockLogger.Object,
                 mockHttpClientFactory.Object, olcConfigSettings);

             // Act
             var result = await sut.EvaluateServiceDefinition(Provider,
                 ServiceDefinitionId,
                 RequestBody,
                 true,
                 "1");

             // Assert
             result.Should().NotBeNull();

             handlerMock.Protected().Verify(
                 "SendAsync",
                 Times.Once(),
                 ItExpr.Is<HttpRequestMessage>(req =>
                     req.Properties.Count == 1
                     && req.Headers.Contains(Constants.HttpHeaders.JavascriptDisabled)
                     && req.Headers.Contains(Constants.OnlineConsultationConstants.ProviderIdentifierHeader)
                 ),
                 ItExpr.IsAny<CancellationToken>()
             );
         }

         [TestMethod]
         public async Task EvaluateServiceDefinitionQuery__AddsOlcSessionIdHeader()
         {
             // Arrange
             var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
             var olcConfigSettings = new OnlineConsultationsConfigurationSettings(30);

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

             var mockLogger = new Mock<ILogger<EvaluateServiceDefinitionQuery>>();
             var mockHttpClientFactory = new Mock<IHttpClientFactory>();
             const string Provider = "ProviderKey";
             const string ServiceDefinitionId = "SOME_SD";
             const string RequestBody = "{\"request\": \"body\"}";
             const string sessionId = "1234-234-123451";

             var httpClient = new HttpClient(handlerMock.Object)
             {
                BaseAddress = new Uri("http://test.com/")
             };

             mockHttpClientFactory.Setup(a => a.CreateClient(Provider))
                 .Returns(httpClient);

             var sut = new EvaluateServiceDefinitionQuery(mockLogger.Object,
                 mockHttpClientFactory.Object, olcConfigSettings);

             // Act
             var result = await sut.EvaluateServiceDefinition(Provider,
                 ServiceDefinitionId,
                 RequestBody,
                 false,
                 "1",
                 sessionId);

             // Assert
             result.Should().NotBeNull();

             handlerMock.Protected().Verify(
                 "SendAsync",
                 Times.Once(),
                 ItExpr.Is<HttpRequestMessage>(req =>
                     req.Properties.Count == 1
                     && req.Headers.Contains(Constants.OnlineConsultationConstants.SessionIdentifierHeader)
                     && req.Headers.Contains(Constants.OnlineConsultationConstants.ProviderIdentifierHeader)
                 ),
                 ItExpr.IsAny<CancellationToken>()
             );
         }
     }
 }