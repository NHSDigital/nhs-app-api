namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class TokenResponse
    {
        public string Access_Token { get; }

        public TokenResponse(string tokenResponse)
        {
            Access_Token = tokenResponse;
        }
    }
}
