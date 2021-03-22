using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.NhsLogin.Fido.Assertion.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal sealed class AuthenticationAssertionBuilder
    {
        private readonly TagUafV1AuthAssertion _assertion = new TagUafV1AuthAssertion();
        private readonly AssertionBuilder<AuthenticationAssertionBuilder> _assertionBuilder;

        public AuthenticationAssertionBuilder(ILogger logger)
        {
            _assertionBuilder = new AssertionBuilder<AuthenticationAssertionBuilder>(logger, this);
        }

        public AuthenticationAssertionBuilder FinalChallengeParams(string fcParams)
            => _assertionBuilder.FinalChallengeParams(_assertion.UafV1SignedData.FinalChallenge, fcParams);

        public AuthenticationAssertionBuilder KeyId(byte[] keyId)
            => _assertionBuilder.KeyId(_assertion.UafV1SignedData.KeyId, keyId);

        public async Task<AuthenticationAssertionBuilder> Sign(Func<byte[], Task<byte[]>> signer)
            => await _assertionBuilder.Sign(signer, _assertion.UafV1SignedData.Write, _assertion.Signature).ResumeOnThreadPool();

        public async Task<string> Build()
            => await _assertionBuilder.Build("Authentication", _assertion).ResumeOnThreadPool();
    }
}