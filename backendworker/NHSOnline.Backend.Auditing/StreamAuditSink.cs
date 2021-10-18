using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Auditing
{
    public class StreamAuditSink : IAuditSink, IDisposable
    {
        private readonly TextWriter _writer;
        private bool _disposed;

        public StreamAuditSink(TextWriter writer)
        {
            _writer = writer;
        }

        public Task WritePreOperationAudit(AuditRecord auditRecord)
        {
            return WriteAudit(auditRecord);
        }

        public Task WritePostOperationAudit(AuditRecord auditRecord)
        {
            return WriteAudit(auditRecord);
        }

        private Task WriteAudit(AuditRecord auditRecord)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            var auditStringBuilder = new StringBuilder(" | ")
                .AppendJoin(" | ",
                    auditRecord.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
                    auditRecord.AuditId,
                    auditRecord.NhsLoginSubject,
                    auditRecord.NhsNumber,
                    auditRecord.IsActingOnBehalfOfAnother,
                    auditRecord.Supplier,
                    auditRecord.Operation,
                    auditRecord.Details)
                .Append(" |");

            if (!string.IsNullOrEmpty(auditRecord.ApiVersion))
            {
                auditStringBuilder.Append($" API Version: {auditRecord.ApiVersion} |");
            }
            if (!string.IsNullOrEmpty(auditRecord.WebVersion))
            {
                auditStringBuilder.Append($" Web Version: {auditRecord.WebVersion} |");
            }
            if (!string.IsNullOrEmpty(auditRecord.NativeVersion))
            {
                auditStringBuilder.Append($" Native Version: {auditRecord.NativeVersion} |");
            }

            _writer.WriteLine(auditStringBuilder.ToString());
            _writer.Flush();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StreamAuditSink()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _writer.Dispose();
            }

            _disposed = true;
        }
    }
}
