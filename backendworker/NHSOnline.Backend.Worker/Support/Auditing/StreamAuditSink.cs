using System;
using System.IO;
using System.Threading.Tasks;

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

        public async Task WriteAudit(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            await _streamWriter.WriteLineAsync($" | {timestamp:yyyy-MM-dd HH:mm:ss.fff} | {nhsNumber} | {supplier} | {operation} | {details} |");
            await _streamWriter.FlushAsync();
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
