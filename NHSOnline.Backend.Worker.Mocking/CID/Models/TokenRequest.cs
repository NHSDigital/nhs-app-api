namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class TokenRequest
    {
        public string Grant_Type { get; }
        public string Code { get; }
        public string Redirect_Uri { get; }
        public string Code_Verifier { get; }
        public string Client_Id { get; }
        public string Code_Challenge_Method { get; }


        public TokenRequest(string code, string code_verifier)
        {
            Grant_Type = "authorization_code";
            Code = code;
            Redirect_Uri = "http://localhost:3000/auth-return";
            Code_Verifier = code_verifier;
            Client_Id = "nhs-online-poc";
            Code_Challenge_Method = "S256";
        }
    }
}


