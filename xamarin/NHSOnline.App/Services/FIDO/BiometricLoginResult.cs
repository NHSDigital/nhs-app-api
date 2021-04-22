namespace NHSOnline.App.Services.FIDO
{
    internal abstract class BiometricLoginResult
    {
        private BiometricLoginResult()
        {
        }

        public abstract T Accept<T>(IBiometricLoginResultVisitor<T> visitor);

        internal sealed class NotRegistered : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Authorised : BiometricLoginResult
        {
            public string FidoAuthResponse { get; }

            public Authorised(string fidoAuthResponse)
            {
                FidoAuthResponse = fidoAuthResponse;
            }

            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Unauthorised : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class UserCancelled : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class SystemCancelled : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failed : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class PermanentLockout : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class TemporaryLockout : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}