using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdSigningKeysProviderTests
    {
        private const string SigningKeyTemplate = @"{{
          ""keys"": [
            {{
              ""kty"": ""RSA"",
              ""kid"": ""{0}"",
              ""e"": ""AQAB"",
              ""n"": ""vYKSjXOcKZI5eNvKT0BuMUAy-N7-f1-88H-Lgz5UOlyAT3wmKNHwwuz11qmovmZaKSTHk94bLIigwGIoc-nsQOahLxS1T-g0R5xN5PRvZUfK6B5W7ONX5EaXDXimKnxQLvIFXJpqzYyStkhYROTuELv70aKQNfYBrb2yZxdPNbjMzSL881awt6wiTIk76kDpzGJ0TcBBrhNKOxPU_L00FT-ASf2mKENTx2QLW8Srgw2SYo3xWhhccz1cEgjllnsX21EYNM95_hcQOBFeDfU7lYEfYGj4bX2mHE4m5up0uLAf5hOIXnfvpmtOKmUizyA9_3yPye1zJpIfZKNgtUo6-Q""
            }}
          ]
        }}";
        private static readonly string TestKid = Guid.NewGuid()
            .ToString()
            .Replace("-", string.Empty, StringComparison.Ordinal);

        private Mock<ICitizenIdClient> _citizenIdClient;

        private CitizenIdSigningKeysProvider _provider;

        private bool _returnHttpErrorResponse;
        private bool _throwHttpException;
        private JsonWebKeySet _signingKey;

        [TestInitialize]
        public void TestInitialize()
        {
            _returnHttpErrorResponse = false;
            _throwHttpException = false;

            BuildKeySet();

            _citizenIdClient = new Mock<ICitizenIdClient>();

            _citizenIdClient
                .Setup(x => x.GetSigningKeys())
                .Returns(() =>
                {
                    if (_throwHttpException)
                    {
                        throw new HttpRequestException();
                    }

                    return Task.FromResult(
                        !_returnHttpErrorResponse
                            ? new CitizenIdApiObjectResponse<JsonWebKeySet>(HttpStatusCode.OK)
                            {
                                Body = _signingKey,
                                ErrorResponse = null
                            }
                            : new CitizenIdApiObjectResponse<JsonWebKeySet>(HttpStatusCode.BadRequest)
                            {
                                Body = null,
                                ErrorResponse = new ErrorResponse { Error = "invalid_grant" }
                            }
                    );
                });

            _provider = new CitizenIdSigningKeysProvider(
                _citizenIdClient.Object, new NullLogger<CitizenIdSigningKeysProvider>());
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_Then_CidKey_IsReturned() =>
            await RunGetSigningKeysTest(Option.Some(_signingKey));

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_CidKey_DoesNot_Match_Then_None_IsReturned() =>
            await RunGetSigningKeysTest(Option.None<JsonWebKeySet>(), "wrong-key");

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_CidClient_Throws_AnException_Then_None_IsReturned()
        {
            _throwHttpException = true;

            await RunGetSigningKeysTest(Option.None<JsonWebKeySet>());
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_CidClient_Returns_AnErrorResponse_Then_None_IsReturned()
        {
            _returnHttpErrorResponse = true;

            await RunGetSigningKeysTest(Option.None<JsonWebKeySet>());
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_IsCalledAgain_ForSameKey_Then_CidClient_IsNot_CalledAgain()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));
            await RunGetSigningKeysTest(Option.Some(_signingKey), expectedCidCallCount: Times.Once);
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_IsCalledAgain_ForAnotherKey_Then_CidClient_Is_CalledAgain()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));

            BuildKeySet("new-key");

            await RunGetSigningKeysTest(Option.Some(_signingKey),
                "new-key",
                () => Times.Exactly(2));
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_IsCalledAgain_ForAnotherKey_And_CidKey_DoesNot_Match_Then_None_IsReturned()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));

            await RunGetSigningKeysTest(Option.None<JsonWebKeySet>(),
                "new-key",
                () => Times.Exactly(2));
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_IsCalledAgain_ForAnotherKey_And_Then_Is_Called_ForSameKey_Then_CidClient_Is_CalledTwice()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));

            BuildKeySet("new-key");

            await RunGetSigningKeysTest(Option.Some(_signingKey),
                "new-key",
                () => Times.Exactly(2));

            await RunGetSigningKeysTest(Option.Some(_signingKey),
                "new-key",
                () => Times.Exactly(2));
        }

        [TestMethod]
        public void When_GetSigningKeys_IsCalled_ForNonCached_Key_In_Parallel_Then_CidClient_Is_Called_Once()
        {
            RunParallelGetSigningKeysTest(() =>
                RunGetSigningKeysTest(Option.Some(_signingKey), expectedCidCallCount: Times.Once));
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_ForCachedKey_In_Parallel_Then_CidClient_Is_Called_Once()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));

            RunParallelGetSigningKeysTest(() =>
                RunGetSigningKeysTest(Option.Some(_signingKey), expectedCidCallCount: Times.Once));
        }

        [TestMethod]
        public async Task When_GetSigningKeys_IsCalled_ForNonCached_Key_And_Then_ForAnotherNonCachedKey_In_Parallel_Then_CidClient_Is_Called_Twice()
        {
            await RunGetSigningKeysTest(Option.Some(_signingKey));

            BuildKeySet("new-key");

            RunParallelGetSigningKeysTest(() =>
                RunGetSigningKeysTest(Option.Some(_signingKey),
                    "new-key",
                    () => Times.Exactly(2)));
        }

        private void BuildKeySet(string keyId = null)
        {
            keyId ??= TestKid;

            _signingKey = new JsonWebKeySet(string.Format(CultureInfo.InvariantCulture, SigningKeyTemplate, keyId));
        }

        private async Task RunGetSigningKeysTest(
            Option<JsonWebKeySet> expectedResult,
            string keyId = null,
            Func<Times> expectedCidCallCount = null)
        {
            keyId ??= TestKid;
            expectedCidCallCount ??= Times.Once;

            var result = await _provider.GetSigningKeys(keyId);

            _citizenIdClient.Verify(x => x.GetSigningKeys(), expectedCidCallCount);

            result.Should().Be(expectedResult);
        }

        private void RunParallelGetSigningKeysTest(Func<Task> test)
        {
            Task.WaitAll(
                Enumerable.Range(1, 20)
                    .Select(_ => test())
                    .ToArray()
            );
        }
    }
}
