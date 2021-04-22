using System;
using Android.Runtime;
using Java.Security;
using Java.Security.Cert;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class BiometricKeyStore
    {
        internal const string InstanceName = "AndroidKeyStore";
        internal const string KeyPrefix = "com.nhs.online.nhsonline.fidouafclient.keystore.key";

        private static readonly Lazy<KeyStore> KeyStoreLazy = new Lazy<KeyStore>(() =>
        {
            var keyStore = KeyStore.GetInstance(InstanceName);
            keyStore?.Load(null);
            return keyStore!;
        });

        private static KeyStore KeyStore =>
            KeyStoreLazy.Value;

        internal static IPrivateKey? GetKey(string name) =>
            KeyStore.GetKey($"{KeyPrefix}_{name}", null).JavaCast<IPrivateKey>();

        internal static Certificate? GetCertificate(string name) =>
            KeyStore.GetCertificate($"{KeyPrefix}_{name}");

        internal static void DeleteEntry(string name) =>
            KeyStore.DeleteEntry($"{KeyPrefix}_{name}");
    }
}