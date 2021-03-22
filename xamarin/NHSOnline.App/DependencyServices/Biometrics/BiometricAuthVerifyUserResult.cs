namespace NHSOnline.App.DependencyServices.Biometrics
{
    public abstract class BiometricAuthVerifyUserResult
    {
        private BiometricAuthVerifyUserResult()
        {
        }

        public abstract T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor);

        public sealed class Authorised : BiometricAuthVerifyUserResult
        {
            public Authorised(IBiometricAuthSigner signer) => Signer = signer;

            public IBiometricAuthSigner Signer { get; }
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Cancelled : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}