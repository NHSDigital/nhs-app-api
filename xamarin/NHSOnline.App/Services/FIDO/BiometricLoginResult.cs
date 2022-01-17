namespace NHSOnline.App.Services.FIDO
{
    internal abstract class BiometricLoginResult
    {
        private BiometricLoginResult()
        {
        }

        public abstract T Accept<T>(IBiometricLoginResultVisitor<T> visitor);

        internal sealed class Authorised : BiometricLoginResult
        {
            public string FidoAuthResponse { get; }

            public Authorised(string fidoAuthResponse)
            {
                FidoAuthResponse = fidoAuthResponse;
            }

            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class CouldNotLogin : BiometricLoginResult
        {
            public CouldNotLoginReason Reason { get; }

            public CouldNotLogin(CouldNotLoginReason reason)
            {
                Reason = reason;
            }

            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class NoAction : BiometricLoginResult
        {
            public NoActionReason Reason { get; }

            public NoAction(NoActionReason reason)
            {
                Reason = reason;
            }

            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failed : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Lockout : BiometricLoginResult
        {
            public LockoutType Type { get; }

            public Lockout(LockoutType type)
            {
                Type = type;
            }
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class LegacySensorNotValid : BiometricLoginResult
        {
            public override T Accept<T>(IBiometricLoginResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}