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
        public void DeserializeSuccessfully_WhenUserSessionIsMissingIm1ConnectionToken()
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
    }
}