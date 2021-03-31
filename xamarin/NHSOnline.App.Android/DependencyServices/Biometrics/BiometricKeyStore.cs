using System;
using Java.Security;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class BiometricKeyStore
    {
        internal const string KeyName = "com.nhs.online.nhsonline.fidouafclient.keystore.key_fidoUsername";

        private static readonly Lazy<KeyStore> KeyStoreLazy = new Lazy<KeyStore>(() =>
        {
            var keyStore = KeyStore.GetInstance("AndroidKeyStore");
            keyStore?.Load(null);
            return keyStore!;
        });

        internal static KeyStore KeyStore => KeyStoreLazy.Value;
    }
}