using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSOnline.MetricLogFunctionApp.Database.EntityFramework;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Load.Postgres
{
    internal sealed class PostgresEventsRepository : IEventsRepository
    {
        private readonly IEtlLogger<PostgresEventsRepository> _etlLogger;
        private readonly TransactionContext _transactionContext;
        public string Env = Environment.GetEnvironmentVariable("QueueNameSuffix");

        private readonly List<string> _runEnvironments = new List<string>
        {
            "dev"
        };

        public PostgresEventsRepository(
            IEtlLogger<PostgresEventsRepository> etlLogger,
            TransactionContext transactionContext)
        {
            _etlLogger = etlLogger;
            _transactionContext = transactionContext;
        }

        public async Task CallStoredProcedure(string call, params object[] parameters)
        {
            await _transactionContext.Database.ExecuteSqlRawAsync(call, parameters);
        }
    }
}