using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Android.Security.Keystore;
using AndroidX.Fragment.App;
using AndroidX.Biometric;
using Java.Lang;
using Java.Math;
using Java.Security;
using Java.Security.Cert;
using Java.Security.Interfaces;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal sealed class BiometricAuthKey : IBiometricAuthKey
    {
        private readonly FragmentActivity _fragmentActivity;
        private readonly IPrivateKey _secretKey;
        private readonly Certificate _certificate;

        private static ILogger Logger => NhsAppLogging.CreateLogger<BiometricAuthKey>();

        public BiometricAuthKey(
            FragmentActivity fragmentActivity,
            IPrivateKey secretKey,
            Certificate certificate)
        {
            _fragmentActivity = fragmentActivity;
            _secretKey = secretKey;
            _certificate = certificate;
        }

        public byte[] PublicKeyEccX962Raw()
        {
            var publicKey = _certificate.PublicKey.JavaCast<IECPublicKey>().ThrowIfNull("Could not cast to IECPublicKey");
            var publicKeyW = publicKey.GetW().ThrowIfNull("GetW returned null");

            using var memoryStream = new MemoryStream(65);
            memoryStream.WriteByte(0x04);
            Append(publicKeyW.AffineX.ThrowIfNull("AffineX returned null"));
            Append(publicKeyW.AffineY.ThrowIfNull("AffineY returned null"));
            return memoryStream.ToArray();

            void Append(BigInteger value)
            {
                var bytes = value.ToByteArray().ThrowIfNull("ToByteArray returned null");
                if (bytes[0] == 0)
                {
                    memoryStream.Write(bytes.AsSpan()[1..]);
                }
                else
                {
                    memoryStream.Write(bytes);
                }
            }
        }

        public async Task<BiometricAuthVerifyUserResult> VerifyUser(string reason)
        {
            var signature = Signature.GetInstance("SHA256withECDSA");
            if (signature == null)
            {
                return new BiometricAuthVerifyUserResult.Unauthorised();
            }

            try
            {
                signature.InitSign(_secretKey);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                Logger.LogError(e, "Failed to init signature. KeyStore value invalidated");
                return new BiometricAuthVerifyUserResult.PermanentLockout();
            }

            using var promptInfoBuilder = new BiometricPrompt.PromptInfo.Builder();
            var promptInfo = promptInfoBuilder
                .SetDescription("Touch the fingerprint sensor on your device")
                .SetTitle("Turn on Fingerprint ID")
                .SetNegativeButtonText("Cancel")
                .Build();

            var completionSource = new TaskCompletionSource<BiometricAuthVerifyUserResult>();

            using var authenticationCallback = new AuthCallback(completionSource);
            using var biometricPrompt = new BiometricPrompt(_fragmentActivity, Application.Context.MainExecutor, authenticationCallback);
            using var cryptoObject = new BiometricPrompt.CryptoObject(signature);

            biometricPrompt.Authenticate(promptInfo, cryptoObject);

            return await completionSource.Task.ResumeOnThreadPool();
        }

        public Task Delete(string fidoUsername)
        {
            BiometricRegistrationState.FidoRegistered = false;

            BiometricKeyStore.DeleteEntry(fidoUsername);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _certificate.Dispose();
            _secretKey.Dispose();
        }

        private class AuthCallback : BiometricPrompt.AuthenticationCallback
        {
            private static ILogger Logger => NhsAppLogging.CreateLogger<AuthCallback>();
            private bool _userPresentedBiometrics;

            private readonly TaskCompletionSource<BiometricAuthVerifyUserResult> _completionSource;

            public AuthCallback(TaskCompletionSource<BiometricAuthVerifyUserResult> completionSource)
            {
                _completionSource = completionSource;
            }

            public override void OnAuthenticationError(int errorCode, ICharSequence errString)
            {
                base.OnAuthenticationError(errorCode, errString);

                switch (errorCode)
                {
                    case BiometricPrompt.ErrorUserCanceled:
                    case BiometricPrompt.ErrorNegativeButton:
                        Logger.LogInformation("Cancelled by user");
                        _completionSource.SetResult(new BiometricAuthVerifyUserResult.UserCancelled());
                        break;

                    case BiometricPrompt.ErrorLockout:
                        if (_userPresentedBiometrics)
                        {
                            Logger.LogInformation("Failed to recognise biometric");
                            _completionSource.SetResult(new BiometricAuthVerifyUserResult.Unauthorised());
                            break;
                        }
                        Logger.LogInformation("Biometric authentication suspended");
                        _completionSource.SetResult(new BiometricAuthVerifyUserResult.TemporaryLockout());
                        break;

                    case BiometricPrompt.ErrorLockoutPermanent:
                        Logger.LogInformation("Sensor disabled. Permanently locked out");
                        _completionSource.SetResult(new BiometricAuthVerifyUserResult.PermanentLockout());
                        break;

                    default:
                        Logger.LogWarning("Authentication error: {ErrorCode} {ErrString}", errorCode, errString);
                        _completionSource.SetResult(new BiometricAuthVerifyUserResult.Unauthorised());
                        break;
                }
            }

            public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
            {
                base.OnAuthenticationSucceeded(result);
                _completionSource.SetResult(new BiometricAuthVerifyUserResult.Authorised(new BiometricAuthSigner(result.CryptoObject)));
            }

            public override void OnAuthenticationFailed()
            {
                base.OnAuthenticationFailed();

                Logger.LogWarning("Authentication failed, biometric presented but not recognised");
                _userPresentedBiometrics = true;
            }
        }
    }
}