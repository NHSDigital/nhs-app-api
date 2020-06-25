namespace NHSOnline.App.NhsLogin
{
    public sealed class ProofKeyCodeExchangeCodes
    {
        internal ProofKeyCodeExchangeCodes(string verifier, string challenge, string method)
        {
            Verifier = verifier;
            Challenge = challenge;
            Method = method;
        }

        public string Verifier { get; }
        public string Challenge { get; }
        public string Method { get; }
    }
}