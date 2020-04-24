namespace NHSOnline.Backend.GpSystems
{
    internal static class TokenValidationServiceExtensions
    {
        internal static bool IsInvalidConnectionTokenFormat(
            this ITokenValidationService tokenValidationService,
            string connectionToken)
            => !tokenValidationService.IsValidConnectionTokenFormat(connectionToken);
    }
}