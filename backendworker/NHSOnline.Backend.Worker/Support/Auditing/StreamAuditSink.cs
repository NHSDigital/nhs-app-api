using System;
using System.IO;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class StreamAuditSink : IAuditSink
    {
        private readonly StreamWriter _streamWriter;
        public StreamAuditSink(Stream stream)
        {
            _streamWriter = new StreamWriter(stream);
        }

        public void WriteAudit(DateTime timestamp, string nhsNumber, string operation, string details)
        {
            _streamWriter.WriteLine($" | {timestamp:yyyy-MM-dd HH:mm:ss.fff} | {nhsNumber} | {operation} | {details} |");
            _streamWriter.Flush();
        }
    }
}
