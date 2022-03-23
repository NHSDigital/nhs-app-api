using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Database.EntityFramework
{
    internal sealed class AadAuthenticationInterceptor : DbConnectionInterceptor
    {
        private const string AccessTokenPlaceholder = "%AccessToken%";

        private readonly ILogger _logger;

        public AadAuthenticationInterceptor(ILogger<AadAuthenticationInterceptor> logger)
        {
            _logger = logger;
        }

        public override InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
            => throw new InvalidOperationException("Open connections asynchronously when using AAD authentication.");

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
        {
            // Assigning to the ConnectionString property has side effects, only do it if we need to.
            var connectionString = connection.ConnectionString;
            if (connectionString.Contains(AccessTokenPlaceholder, StringComparison.OrdinalIgnoreCase))
            {
                connection.ConnectionString = await ReplaceAccessTokenPlaceholder(connectionString, cancellationToken);
            }

            return result;
        }

        private async Task<string> ReplaceAccessTokenPlaceholder(
            string connectionString,
            CancellationToken cancellationToken)
        {
            var accessToken = await FetchAccessToken(cancellationToken);

            return connectionString.Replace(AccessTokenPlaceholder, accessToken, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> FetchAccessToken(CancellationToken cancellationToken)
        {
            var provider = new AzureServiceTokenProvider();

            var accessToken = await provider.GetAccessTokenAsync(
                "https://ossrdbms-aad.database.windows.net",
                cancellationToken: cancellationToken);

            if (accessToken is null)
            {
                _logger.LogWarning("Null access token returned");
                throw new InvalidOperationException("Null access token returned");
            }

            _logger.LogInformation("Retrieved access token of length {AccessTokenLength}", accessToken.Length);
            return accessToken;
        }
    }
}