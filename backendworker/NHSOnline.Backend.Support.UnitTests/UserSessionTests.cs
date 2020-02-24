using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;

namespace NHSOnline.Backend.Support.UnitTests
{

    [TestClass]
    public class UserSessionTests
    {
        private IFixture _fixture;
        private JsonSerializerSettings _serializerSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        [TestMethod]
        public void SerializeSuccessfully_WhenUserSessionIsMissingIm1ConnectionToken()
        {
            //Arrange
            var userSession = _fixture.Freeze<UserSession>();
            userSession.GpUserSession = new EmisUserSession();
            userSession.Im1ConnectionToken = null;

            //Act
            var json = JsonConvert.SerializeObject(userSession, _serializerSettings);

            //Assert
            Assert.IsNotNull(json);
        }

        [TestMethod]
        public void DeserializeSuccessfully_WhenEmisUserSessionIsMissingIm1ConnectionToken()
        {
            //Arrange
            const string json =
                "{" +
                "\"Key\"          : \"myKey\"," +
                "\"CsrfToken\"    : \"myToken\"," +
                "\"GpUserSession\": {" +
                    "\"$type\":\"NHSOnline.Backend.GpSystems.Suppliers.Emis.EmisUserSession, NHSOnline.Backend.GpSystems\"," +
                    "\"Supplier\"                             : 1," +
                    "\"SessionId\"                            : \"mySessionId\"," +
                    "\"EndUserSessionId\"                     : null," +
                    "\"UserPatientLinkToken\"                 : null," +
                    "\"AppointmentBookingReasonNecessity\"    : 0," +
                    "\"PrescriptionSpecialRequestNecessity\"  : 0," +
                    "\"ProxyPatients\"                        : null," +
                    "\"Name\"                                 : null," +
                    "\"NhsNumber\"                            : null," +
                    "\"OdsCode\"                              : null," +
                    "\"HasLinkedAccounts\"                    : false," +
                    "\"Im1MessagingEnabled\"                  : false," +
                    "\"Id\"                                   : \"00000000-0000-0000-0000-000000000000\"" +
                "}," +
                "\"CitizenIdUserSession\":{" +
                    "\"AccessToken\"  : \"myAccessToken\"," +
                    "\"FamilyName\"   : \"myFamilyName\"," +
                    "\"DateOfBirth\"  : \"03/03/2000 19:28:20\"," +
                    "\"IdTokenJti\"   : \"myIdTokenJti\"" +
                "}," +
                "\"OrganDonationSessionId\" : \"00000000-0000-0000-0000-000000000000\"," +
            "}";

            //Act
            var userSession = JsonConvert.DeserializeObject<UserSession>(json, _serializerSettings);

            //Assert
            Assert.IsNotNull(userSession);
            Assert.IsNull(userSession.Im1ConnectionToken);
        }

        [TestMethod]
        public void DeserializeSuccessfully_WhenTppUserSessionIsMissingPatientNode()
        {
            //Arrange
            const string json =
                "{" +
                "\"$type\":\"NHSOnline.Backend.Support.UserSession, NHSOnline.Backend.Support\"," +
                "\"Key\":\"Keyf5687d2c-5857-450b-8625-a8b4c5b84068\"," +
                "\"CsrfToken\":\"CsrfTokend3aea227-7dcf-47ac-bf7c-5a61a8870dc9\"," +
                "\"GpUserSession\":{" +
                "\"$type\":\"NHSOnline.Backend.GpSystems.Suppliers.Tpp.TppUserSession, NHSOnline.Backend.GpSystems\"," +
                "\"Supplier\":2," +
                "\"HasLinkedAccounts\":true," +
                "\"Suid\":null," +
                "\"PatientId\":null," +
                "\"OnlineUserId\":null," +
                "\"UnitId\":null," +
                "\"Name\":null," +
                "\"NhsNumber\":null," +
                "\"OdsCode\":null," +
                "\"Im1MessagingEnabled\":false," +
                "\"Id\":\"00000000-0000-0000-0000-000000000000\"" +
                "}," +
                "\"CitizenIdUserSession\":{" +
                "\"$type\":\"NHSOnline.Backend.Support.CitizenIdUserSession, NHSOnline.Backend.Support\"," +
                "\"AccessToken\":\"AccessTokend9db6c38-f1c4-4231-81d7-181968250edb\"," +
                "\"FamilyName\":\"FamilyName8148b3f2-ee75-405b-8560-846bf35b2e20\"," +
                "\"DateOfBirth\":\"2021-01-25T00:13:15.961268\"," +
                "\"IdTokenJti\":\"IdTokenJtic7829065-b3aa-4504-8788-56d0d1dc6143\"" +
                "}," +
                "\"OrganDonationSessionId\":\"0495a20c-228e-44fd-9988-e6e0834d75c2\"," +
                "\"Im1ConnectionToken\":\"This is a Connection Token\"" +
                "}";

            //Act
            var userSession = JsonConvert.DeserializeObject<UserSession>(json, _serializerSettings);

            //Assert
            Assert.IsNotNull(userSession);
        }



    }
}