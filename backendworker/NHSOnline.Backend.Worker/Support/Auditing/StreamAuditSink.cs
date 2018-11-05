using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class StreamAuditSink : IAuditSink, IDisposable
    {
        private readonly StreamWriter _streamWriter;
        private bool _disposed;
        
        public StreamAuditSink(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);
        }

        public Task WriteAudit(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details, VersionTag versionTag)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            var auditStringBuilder = new StringBuilder();
            auditStringBuilder.Append($" | {timestamp:yyyy-MM-dd HH:mm:ss.fff} | {nhsNumber} | {supplier} | {operation} | {details} |");

            if (versionTag != null)
            {
                if (!string.IsNullOrEmpty(versionTag.Api))
                {
                    auditStringBuilder.Append($" API Version: {versionTag.Api} |");
                }
                if (!string.IsNullOrEmpty(versionTag.Web))
                {
                    auditStringBuilder.Append($" Web Version: {versionTag.Web} |");
                }
                if (!string.IsNullOrEmpty(versionTag.Native))
                {
                    auditStringBuilder.Append($" Native Version: {versionTag.Native} |");
                }                
            }

            _streamWriter.WriteLine(auditStringBuilder.ToString());
            _streamWriter.Flush();

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
                _streamWriter.Dispose();
            }

            _disposed = true;
        }
    }
}
