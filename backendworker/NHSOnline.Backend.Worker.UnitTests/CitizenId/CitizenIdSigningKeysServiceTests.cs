using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdSigningKeysServiceTests
    {
        private IFixture _fixture;
        private CitizenIdSigningKeysService _systemUnderTest;
        private Mock<ICitizenIdClient> _citizenIdClientMock;
        private string _signingKey =
            "{\"keys\": [{\"kty\": \"RSA\", \"e\": \"AQAB\", \"n\": \"vYKSjXOcKZI5eNvKT0BuMUAy-N7-f1-88H-Lgz5UOlyAT3wmKNHwwuz11qmovmZaKSTHk94bLIigwGIoc-nsQOahLxS1T-g0R5xN5PRvZUfK6B5W7ONX5EaXDXimKnxQLvIFXJpqzYyStkhYROTuELv70aKQNfYBrb2yZxdPNbjMzSL881awt6wiTIk76kDpzGJ0TcBBrhNKOxPU_L00FT-ASf2mKENTx2QLW8Srgw2SYo3xWhhccz1cEgjllnsX21EYNM95_hcQOBFeDfU7lYEfYGj4bX2mHE4m5up0uLAf5hOIXnfvpmtOKmUizyA9_3yPye1zJpIfZKNgtUo6-Q\"}]}";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _citizenIdClientMock = _fixture.Freeze<Mock<ICitizenIdClient>>();
            _systemUnderTest = _fixture.Create<CitizenIdSigningKeysService>();
        }

        [TestMethod]
        public async Task GetSigningKeys_GetSigningKeysSuccesfull_ReturnsValidSigningKeys()
        {
            //Arrange
            var signingKeys = new JsonWebKeySet(_signingKey);
            var expectedResponse = Option.Some(signingKeys);
            
            var signingKeysResponse = new CitizenIdClient.CitizenIdApiObjectResponse<JsonWebKeySet>(HttpStatusCode.OK)
            {
                Body = signingKeys,
                ErrorResponse = null
            };
            
            _citizenIdClientMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);
            
            //Act
            var result = await _systemUnderTest.GetSigningKeys();
            
            //Assert
            _citizenIdClientMock.VerifyAll();
            result.Should().Be(expectedResponse);
        }
        
        [TestMethod]
        public async Task GetSigningKeys_GetSigningKeysFails_ReturnsNone()
        {
            //Arrange
            var expectedResult = Option.None<JsonWebKeySet>();
            
            var signingKeysResponse = new CitizenIdClient.CitizenIdApiObjectResponse<JsonWebKeySet>(HttpStatusCode.BadRequest)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant" }
            };
            
            _citizenIdClientMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            //Act
            var result = await _systemUnderTest.GetSigningKeys();
            
            //Assert
            _citizenIdClientMock.VerifyAll();
            result.Should().Be(expectedResult);
        }
        
        [TestMethod]
        public async Task GetSigningKeys_GetSigningKeysSucceedsWithInvalid_ReturnsNone()
        {
            //Arrange
            var expectedResult = Option.None<JsonWebKeySet>();
            
            var signingKeysResponse = new CitizenIdClient.CitizenIdApiObjectResponse<JsonWebKeySet>(HttpStatusCode.BadRequest)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant" }
            };
            
            _citizenIdClientMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            //Act
            var result = await _systemUnderTest.GetSigningKeys();
            
            //Assert
            _citizenIdClientMock.VerifyAll();
            result.Should().Be(expectedResult);
        }
    }
}