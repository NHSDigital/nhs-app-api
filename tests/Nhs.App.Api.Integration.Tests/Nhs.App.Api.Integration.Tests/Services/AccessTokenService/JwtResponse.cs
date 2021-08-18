namespace Nhs.App.Api.Integration.Tests.Services.AccessTokenService
{
    public class JwtResponse
    {
        public string Token { get; set; } = string.Empty;
        public long ExpiresAt { get; set; }
    }
}
