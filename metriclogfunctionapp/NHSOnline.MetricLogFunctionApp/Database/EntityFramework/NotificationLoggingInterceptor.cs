using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace NHSOnline.MetricLogFunctionApp.Database.EntityFramework
{
    internal sealed class NotificationLoggingInterceptor : IDbConnectionInterceptor
    {
        private readonly ILogger _logger;

        public NotificationLoggingInterceptor(ILogger<NotificationLoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
        {
            AddEventHandler(connection);

            return result;
        }

        public ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            AddEventHandler(connection);

            return ValueTask.FromResult(result);
        }

        public void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        { }

        public Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
            => Task.CompletedTask;

        public InterceptionResult ConnectionClosing(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
            => result;

        public ValueTask<InterceptionResult> ConnectionClosingAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
            => ValueTask.FromResult(result);

        public void ConnectionClosed(
            DbConnection connection,
            ConnectionEndEventData eventData)
            => RemoveEventHandler(connection);

        public Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
            => RemoveEventHandlerAsync(connection);

        public void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
            => RemoveEventHandler(connection);

        public Task ConnectionFailedAsync(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
            => RemoveEventHandlerAsync(connection);

        private void AddEventHandler(DbConnection connection)
        {
            if (connection is NpgsqlConnection npgsqlConnection)
            {
                npgsqlConnection.Notice += NpgsqlConnectionOnNotice;
            }
        }

        private Task RemoveEventHandlerAsync(DbConnection connection)
        {
            RemoveEventHandler(connection);

            return Task.CompletedTask;
        }

        private void RemoveEventHandler(DbConnection connection)
        {
            if (connection is NpgsqlConnection npgsqlConnection)
            {
                npgsqlConnection.Notice -= NpgsqlConnectionOnNotice;
            }
        }

        private void NpgsqlConnectionOnNotice(object sender, NpgsqlNoticeEventArgs e)
        {
            var logLevel = e.Notice.Severity switch
            {
                "WARNING" => LogLevel.Warning,
                "DEBUG" => LogLevel.Debug,
                _ => LogLevel.Information
            };

            _logger.Log(
                logLevel,
                "{Severity}: {Message}",
                e.Notice.Severity,
                e.Notice.MessageText);
        }
    }
}