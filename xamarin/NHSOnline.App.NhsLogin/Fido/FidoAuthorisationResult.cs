namespace NHSOnline.App.NhsLogin.Fido
{
    public abstract class FidoAuthorisationResult
    {
        private FidoAuthorisationResult(){}

        public abstract T Accept<T>(IFidoAuthorisationResultVisitor<T> visitor);

        public sealed class Authorised : FidoAuthorisationResult
        {
            internal Authorised(string fidoAuthResponse)
            {
                FidoAuthResponse = fidoAuthResponse;
            }

            public string FidoAuthResponse { get; }
            public override T Accept<T>(IFidoAuthorisationResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public sealed class Unauthorised : FidoAuthorisationResult
        {
            public override T Accept<T>(IFidoAuthorisationResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}