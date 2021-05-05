namespace NHSOnline.App.NhsLogin.Fido
{
    public abstract class FidoRegisterResult
    {
        private FidoRegisterResult() { }

        public abstract T Accept<T>(IFidoRegisterResultVisitor<T> visitor);

        public sealed class Registered : FidoRegisterResult
        {
            public string Username { get; }

            public Registered(string username)
            {
                Username = username;
            }

            public override T Accept<T>(IFidoRegisterResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : FidoRegisterResult
        {
            public override T Accept<T>(IFidoRegisterResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}