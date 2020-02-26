using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client;
using System.Xml.Linq;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppClientPatientSelectedPostTests : IDisposable
    {
        private TppClientTestsContext Context { get; set; }
        private ITppClientRequest<TppUserSession, PatientSelectedReply> SystemUnderTest { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;
        private Mock<ILogger<TppClientRequestExecutor>> MockLogger => Context.MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
            SystemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequest<TppUserSession, PatientSelectedReply>>();
        }

        [TestMethod]
        public async Task PatientSelectedPostRequest_SendsPostRequest_WhenCalled()
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            HttpRequestMessage requestMessage = null;
            CaptureHttpRequestMessage(req => requestMessage = req);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            requestMessage.Method.Should().Be(HttpMethod.Post);
        }

        [TestMethod]
        public async Task PatientSelectedPostRequest_UsesTppApiUrl_WhenCalled()
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            HttpRequestMessage requestMessage = null;
            CaptureHttpRequestMessage(req => requestMessage = req);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            requestMessage.RequestUri.Should().Be(TppClientTestsContext.ApiUrl);
        }

        [TestMethod]
        public async Task PatientSelectedPostRequest_ContainsPatientSelectedRequestTypeHeader_WhenCalled()
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            HttpRequestMessage requestMessage = null;
            CaptureHttpRequestMessage(req => requestMessage = req);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            requestMessage.Headers.Should().ContainHeader("type", "PatientSelected");
        }

        [TestMethod]
        [DataRow("suid")]
        [DataRow("SUID2")]
        public async Task PatientSelectedPostRequest_ContainsPatientSelectedRequestSuidHeader_WhenCalled(string suidHeaderName)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession(suid: suidHeaderName);

            HttpRequestMessage requestMessage = null;
            CaptureHttpRequestMessage(req => requestMessage = req);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            requestMessage.Headers.Should().ContainHeader("suid", suidHeaderName);
        }

        [TestMethod]
        [DataRow("patient id")]
        [DataRow("Patient Id 2")]
        public async Task PatientSelectedPostRequest_ContainsPatientSelectedContentPatientIdAttribute_WhenCalled(string patientId)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession(patientId: patientId);

            string requestContent = null;
            CaptureHttpRequestMessageContent(reqContent => requestContent = reqContent);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            var xmlDocument = XDocument.Parse(requestContent);
            xmlDocument.Should().HaveRoot("PatientSelected").Which.Should().HaveAttribute("patientId", patientId);
        }

        [TestMethod]
        [DataRow("online user id")]
        [DataRow("Online User Id 2")]
        public async Task PatientSelectedPostRequest_ContainsPatientSelectedContentOnlineUserIdAttribute_WhenCalled(string onlineUserId)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession(onlineUserId: onlineUserId);

            string requestContent = null;
            CaptureHttpRequestMessageContent(reqContent => requestContent = reqContent);

            //Act
            await SystemUnderTest.Post(tppUserSession);

            //Assert
            var xmlDocument = XDocument.Parse(requestContent);
            xmlDocument.Should().HaveRoot("PatientSelected").Which.Should().HaveAttribute("onlineUserId", onlineUserId);
        }

        [TestMethod]
        public async Task PatientSelectedPostResponse_ReceivesOkResponse_WhenCalled()
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage();

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.HasSuccessResponse.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("suid", "Suid")]
        [DataRow("suid", "Suid 2")]
        public async Task PatientSelectedPostResponse_ReceivesSuidResponseHeader_WhenCalled(string name, string value)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(httpResponseMessage: message => message.AddHeader(name, value));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Headers.Should().Contain(name, value);
        }

        [TestMethod]
        [DataRow("Patient ID")]
        [DataRow("Patient id 2")]
        public async Task PatientSelectedPostResponse_ReceivesPatientIdResponseContent_WhenCalled(string patientId)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.PatientId(patientId));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.PatientId.Should().Be(patientId);
        }

        [TestMethod]
        [DataRow("2020-02-24", 2020, 2, 24)]
        [DataRow("2019-10-01", 2019, 10, 1)]
        public async Task PatientSelectedPostResponse_ReceivesDateOfBirthResponseContent_WhenCalled(string dateOfBirth, int year, int month, int day)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.DateOfBirth(dateOfBirth));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.DateOfBirth.Should().Be(new DateTime(year, month, day));
        }

        [TestMethod]
        [DataRow("Female")]
        [DataRow("Male")]
        public async Task PatientSelectedPostResponse_ReceiveGenderResponseContent_WhenCalled(string gender)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.Gender(gender));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.Gender.Should().Be(gender);
        }

        [TestMethod]
        [DataRow("National Id Type")]
        [DataRow("NationalID type")]
        public async Task PatientSelectedPostResponse_ReceiveNationalIdTypeResponseContent_WhenCalled(string nationIdType)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.NationalIdType(nationIdType));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.NationalId.Type.Should().Be(nationIdType);
        }

        [TestMethod]
        [DataRow("NationalId Value")]
        [DataRow("NationalIDvalue")]
        public async Task PatientSelectedPostResponse_ReceiveNationalIdValueResponseContent_WhenCalled(string nationIdValue)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.NationalIdValue(nationIdValue));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.NationalId.Value.Should().Be(nationIdValue);
        }

        [TestMethod]
        [DataRow("Person name")]
        [DataRow("Person Name 2")]
        public async Task PatientSelectedPostResponse_ReceivePersonNameResponseContent_WhenCalled(string personName)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.PersonName(personName));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.PersonName.Name.Should().Be(personName);
        }

        [TestMethod]
        [DataRow("Tpp Address")]
        [DataRow("Tpp address 2")]
        public async Task PatientSelectedPostResponse_ReceiveTppAddressResponseContent_WhenCalled(string tppAddress)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.TppAddress(tppAddress));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.Address.Address.Should().Be(tppAddress);
        }

        [TestMethod]
        [DataRow("Tpp Address Type")]
        [DataRow("Tpp address type 2")]
        public async Task PatientSelectedPostResponse_ReceiveTppAddressTypeResponseContent_WhenCalled(string tppAddressType)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(reply => reply.TppAddressType(tppAddressType));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.Body.Person.Address.AddressType.Should().Be(tppAddressType);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task PatientSelectedPostResponse_ReceiveErrorStatusCode_WhenCalled(HttpStatusCode statusCode)
        {
            //Arrange
            var tppUserSession = CreateTppUserSession();

            ArrangeHttpResponseMessage(httpResponseMessage: message => message.SetStatusCode(statusCode));

            //Act
            var response = await SystemUnderTest.Post(tppUserSession);

            //Assert
            response.StatusCode.Should().Be(statusCode);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context.Dispose();
        }

        private TppUserSession CreateTppUserSession(
            string suid = "suid",
            string patientId = "patient id",
            string onlineUserId = "online user id")
            => new TppUserSession
            {
                Suid = suid,
                PatientId = patientId,
                OnlineUserId = onlineUserId
            };

        private void CaptureHttpRequestMessage(Action<HttpRequestMessage> onRequestMessage)
            => CaptureHttpRequestMessage(req =>
            {
                onRequestMessage(req);
                return Task.CompletedTask;
            });

        private void CaptureHttpRequestMessageContent(Action<string> onRequestContent)
            => CaptureHttpRequestMessage(async req => onRequestContent(await req.Content.ReadAsStringAsync()));

        private void CaptureHttpRequestMessage(Func<HttpRequestMessage, Task> onRequestMessage)
        {
            MockHttpHandler
                .When("*")
                .Respond(async req =>
                {
                    var expectedPatientSelectedResponse = new PatientSelectedReplyBuilder().BuildXml();
                    await onRequestMessage(req);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                        .StringContent(expectedPatientSelectedResponse);
                });
        }

        private void ArrangeHttpResponseMessage(
            Func<PatientSelectedReplyBuilder, PatientSelectedReplyBuilder> patientSelectedReplyBuilder = null,
            Func<HttpResponseMessage, HttpResponseMessage> httpResponseMessage = null)
        {
            patientSelectedReplyBuilder = patientSelectedReplyBuilder ?? (b => b);
            httpResponseMessage = httpResponseMessage ?? (b => b);

            MockHttpHandler
               .When("*")
               .Respond(_ =>
               {
                   var expectedPatientSelectedResponse = patientSelectedReplyBuilder(new PatientSelectedReplyBuilder()).BuildXml();
                   return httpResponseMessage(new HttpResponseMessage(HttpStatusCode.OK))
                   .StringContent(expectedPatientSelectedResponse)
                   .ReturnTask();
               });
        }
    }
}