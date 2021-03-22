using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.NhsLogin.Fido.Assertion.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal sealed class RegistrationAssertionBuilder
    {
        private readonly TagUafV1RegAssertion _assertion = new TagUafV1RegAssertion();
        private readonly AssertionBuilder<RegistrationAssertionBuilder> _assertionBuilder;

        public RegistrationAssertionBuilder(ILogger logger)
        {
            _assertionBuilder = new AssertionBuilder<RegistrationAssertionBuilder>(logger, this);
        }

        public RegistrationAssertionBuilder FinalChallengeParams(string fcParams)
            => _assertionBuilder.FinalChallengeParams(_assertion.UafV1KeyRegistrationData.FinalChallenge, fcParams);

        public RegistrationAssertionBuilder KeyId(byte[] keyId)
            => _assertionBuilder.KeyId(_assertion.UafV1KeyRegistrationData.KeyId, keyId);

        public RegistrationAssertionBuilder PublicKeyEccX962Raw(byte[] publicKey)
        {
            _assertion.UafV1KeyRegistrationData.PubKey.KeyBytes(publicKey);
            return this;
        }

        public async Task<RegistrationAssertionBuilder> Sign(Func<byte[], Task<byte[]>> signer)
        {
            return await _assertionBuilder
                .Sign(signer, _assertion.UafV1KeyRegistrationData.Write, _assertion.AttestationBasicFull.Signature)
                .ResumeOnThreadPool();
        }

        public async Task<string> Build()
        {
            return await _assertionBuilder
                .Build("Registration", _assertion)
                .ResumeOnThreadPool();
        }
    }
}