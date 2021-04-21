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

        public sealed class UserCancelled : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class SystemCancelled : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Unauthorised : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class LockedOut : BiometricAuthVerifyUserResult
        {
            public override T Accept<T>(IBiometricAuthVerifyUserResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}