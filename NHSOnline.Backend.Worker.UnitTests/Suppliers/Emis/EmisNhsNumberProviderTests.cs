using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Suppliers.Emis;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisNhsNumberProviderTests
    {
        private const string DefaultEndUserSessionId = "DW3EUerDy8VEZi2gvJ5esg";
        private const string DefaultConnectionToken = "token";
        private const string DefaultSessionId = "session id";
        private const string DefaultOdsCode = "ods code";
        private const string DefaultUserPatientLinkToken = "link token";
        private const string DefaultIdentifierValue = "identifier";

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnNhsNumber_WhenRequested()
        {
            const string expectedNhsNumber = "AB123";

            var expectedNhsNumbers = new [] { expectedNhsNumber };
            var userPatientLinkModels = new [] { CreateUserPatientLinkModel() };
            var patientIdentifiers = new [] { CreatePatientIdentifier(expectedNhsNumber) };
            var emisClientMock = new Mock<IEmisClient>();

            SetupEmisClientMock(
                emisClientMock,
                userPatientLinkModels: userPatientLinkModels,
                patientIdentifiers: patientIdentifiers
            );

            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            var resultantNhsNumbers = result.Select(x => x.NhsNumber).ToArray();
            CollectionAssert.AreEquivalent(expectedNhsNumbers, resultantNhsNumbers);
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnNhsNumberUsingTheSelfUserPatientLinkToken_WhenTheMultipleUserPatientLinkTokensAreReturnedFromEmis()
        {
            const string expectedNhsNumber = "345";
            var emisClientMock = new Mock<IEmisClient>();
            var userPatientLinkModels = new[] {
                CreateUserPatientLinkModel("proxy", AssociationType.Proxy),
                CreateUserPatientLinkModel("self", AssociationType.Self),
            };
            var patientIdentifiers = new[] { CreatePatientIdentifier(expectedNhsNumber) };

            SetupEmisClientMock(
                emisClientMock,
                patientIdentifiers: patientIdentifiers,
                userPatientLinkModels: userPatientLinkModels,
                userPatientLinkToken: "self"
            );

            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            Assert.AreEqual(expectedNhsNumber, result.First().NhsNumber);
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsThePatientNhsNumbersOfTypeNhsNumber_WhenTheMultipleNhsNumbersAreReturnedFromEmis()
        {
            var expectedNhsNumbers = new [] { "1234", "345" };
            var emisClientMock = new Mock<IEmisClient>();
            var userPatientLinkModels = new[] { CreateUserPatientLinkModel() };

            var patientIdentifiers = new[]
            {
                CreatePatientIdentifier("boo", IdentifierType.ChiNumber),
                CreatePatientIdentifier("hoo", IdentifierType.Unknown),
                CreatePatientIdentifier(expectedNhsNumbers[0]),
                CreatePatientIdentifier(expectedNhsNumbers[1]),
            };

            SetupEmisClientMock(
                emisClientMock,
                patientIdentifiers: patientIdentifiers,
                userPatientLinkModels: userPatientLinkModels
            );

            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            CollectionAssert.AreEquivalent(expectedNhsNumbers, result.Select(x => x.NhsNumber).ToArray());
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnEmptyObject_WhenEmisReturnsEmptyPatientIdentifiers()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);
            var userPatientLinkModels = new[] { CreateUserPatientLinkModel() };
            var patientIdentifiers = new PatientIdentifier[0];

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels, patientIdentifiers: patientIdentifiers);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnEmptyObject_WhenEmisReturnsNullPatientIdentifiers()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);
            var userPatientLinkModels = new[] { CreateUserPatientLinkModel() };
            var patientIdentifiers = (PatientIdentifier[]) null;

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels, patientIdentifiers: patientIdentifiers);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnEmptyObject_WhenEmisReturnsEmptyUserLinkModels()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);
            var userPatientLinkModels = new UserPatientLinkModel[0];

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetNhsNumbersAsync_ReturnsAnEmptyObject_WhenEmisReturnsNullUserLinkModels()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var nhsNumberProvider = new EmisNhsNumberProvider(emisClientMock.Object);
            var userPatientLinkModels = (UserPatientLinkModel[]) null;

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels);

            var result = await nhsNumberProvider.GetNhsNumbersAsync(DefaultConnectionToken, DefaultOdsCode);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        private static PatientIdentifier CreatePatientIdentifier(
            string identifierValue = DefaultIdentifierValue,
            IdentifierType identifierType = IdentifierType.NhsNumber
        )
        {
            return new PatientIdentifier
            {
                IdentifierType = identifierType,
                IdentifierValue = identifierValue
            };
        }
        private static UserPatientLinkModel CreateUserPatientLinkModel(
            string userPatientLinkToken = DefaultUserPatientLinkToken,
            AssociationType associationType = AssociationType.Self
        )
        {
            return new UserPatientLinkModel
            {
                UserPatientLinkToken = userPatientLinkToken,
                AssociationType = associationType
            };
        }

        private static void SetupEmisClientMock(
            Mock<IEmisClient> emisClientMock,
            string endUserSessionId = DefaultEndUserSessionId,
            string sessionId = DefaultSessionId,
            string connectionToken = DefaultConnectionToken,
            string odsCode = DefaultOdsCode,
            string userPatientLinkToken = null,
            IEnumerable<UserPatientLinkModel> userPatientLinkModels = null,
            IEnumerable<PatientIdentifier> patientIdentifiers = null
        )
        {
            userPatientLinkToken = userPatientLinkToken ?? userPatientLinkModels?.FirstOrDefault()?.UserPatientLinkToken;

            var endUserSessionResponse = new CreateEndUserSessionResponseModel
            {
                EndUserSessionId = endUserSessionId
            };

            var sessionResponse = new CreateSessionResponseModel
            {
                SessionId = sessionId,
                UserPatientLinks = userPatientLinkModels
            };

            var demographicsResponse = new DemographicsResponse
            {
                PatientIdentifiers = patientIdentifiers
            };

            emisClientMock
                .Setup(x => x.EndUserSessionAsync())
                .ReturnsAsync(endUserSessionResponse);

            emisClientMock
                .Setup(x => x.SessionsAsync(endUserSessionId, connectionToken, odsCode))
                .ReturnsAsync(sessionResponse);

            emisClientMock
                .Setup(x => x.DemographicsAsync(userPatientLinkToken, sessionResponse.SessionId,endUserSessionResponse.EndUserSessionId))
                .ReturnsAsync(demographicsResponse);

            emisClientMock
                .Setup(x => x.SessionsAsync(endUserSessionId, connectionToken, odsCode))
                .ReturnsAsync(sessionResponse);


            emisClientMock
                .Setup(x => x.DemographicsAsync(userPatientLinkToken, sessionResponse.SessionId, endUserSessionId))
                .ReturnsAsync(demographicsResponse);
        }
    }
}
