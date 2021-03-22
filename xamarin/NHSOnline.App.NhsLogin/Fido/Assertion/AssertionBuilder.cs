using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.NhsLogin.Fido.Assertion.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido.Assertion
{
    internal sealed class AssertionBuilder<TBuilder>
    {
        private readonly ILogger _logger;
        private readonly TBuilder _builder;

        public AssertionBuilder(
            ILogger logger,
            TBuilder builder)
        {
            _logger = logger;
            _builder = builder;
        }

        internal TBuilder FinalChallengeParams(TagFinalChallenge finalChallenge, string fcParams)
        {
            using var sha256 = SHA256.Create();

            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(fcParams));
            finalChallenge.Hash(hash);

            return _builder;
        }

        internal TBuilder KeyId(TagKeyId keyId, byte[] keyIdBytes)
        {
            var keyIdString = Base64Url.Encode(keyIdBytes);

            _logger.LogDebug("KeyId: {KeyId}", keyIdString);
            keyId.Value = keyIdString;

            return _builder;
        }

        internal async Task<TBuilder> Sign(
            Func<byte[], Task<byte[]>> signer,
            Action<ITagLengthValueWriter> bytesToSignWriter,
            TagSignature signature)
        {
            await using var bytesToSign = new TagLengthValueWriter();

            bytesToSignWriter(bytesToSign);

            var signatureBytes = await signer(bytesToSign.ToArray()).ResumeOnThreadPool();
            signature.SigBytes(signatureBytes);

            return _builder;
        }

        internal async Task<string> Build(string assertionDesc, ITagAssertion assertion)
        {
            var userVerificationExtension = new UserVerificationExtension();

            _logger.LogTrace("{AssertionType} Assertion: {Assertion}", assertionDesc, assertion);
            _logger.LogTrace("User Verification Extension: {UserVerificationExtension}", userVerificationExtension);

            await using var assertionWriter = new TagLengthValueWriter();

            assertion.Write(assertionWriter);
            userVerificationExtension.Write(assertionWriter);

            return Base64Url.Encode(assertionWriter.ToArray());
        }
    }
}