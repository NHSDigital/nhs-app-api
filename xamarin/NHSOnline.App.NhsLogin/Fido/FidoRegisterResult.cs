namespace NHSOnline.App.NhsLogin.Fido
{
    public abstract class FidoRegisterResult
    {
        private FidoRegisterResult() { }

        public abstract T Accept<T>(IFidoRegisterResultVisitor<T> visitor);

        public sealed class Registered : FidoRegisterResult
        {
            public override T Accept<T>(IFidoRegisterResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Failed : FidoRegisterResult
        {
            public override T Accept<T>(IFidoRegisterResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}