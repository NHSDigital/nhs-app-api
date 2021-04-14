namespace NHSOnline.App.Services.FIDO
{
    internal abstract class BiometricLoginResult
    {
        private BiometricLoginResult()
        {
        }

        public abstract T Accept<T>(IBiometricLoginResultVisitor<T> visitor);

        internal sealed class LoggedIn : BiometricLoginResult
        {
            public string FidoAuthResponse { get; }

            public LoggedIn(string fidoAuthResponse)
            {
                FidoAuthResponse = fidoAuthResponse;
            }

            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Cancelled : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failed : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class NotRegistered : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Invalidated : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}