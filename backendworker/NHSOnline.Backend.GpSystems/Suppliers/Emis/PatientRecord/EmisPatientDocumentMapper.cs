using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Sanitization;


namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
  public class EmisPatientDocumentMapper
  {
    private readonly ILogger<EmisPatientDocumentMapper> _logger;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public EmisPatientDocumentMapper(ILogger<EmisPatientDocumentMapper> logger, IHtmlSanitizer htmlSanitizer)
    {
      _logger = logger;
      _htmlSanitizer = htmlSanitizer;
    }

    public PatientDocument Map(IndividualDocument documentGetResponse)
    {
      _logger.LogInformation("Mapping document response");
      if (documentGetResponse == null)
      {
        throw new ArgumentNullException(nameof(documentGetResponse));
      }

      var document = new GpSystems.PatientRecord.Models.PatientDocument();

      if (documentGetResponse.CompressedEncodedDocumentContent != null)
      {
        var encodedAndCompressedContent = documentGetResponse.CompressedEncodedDocumentContent;

        _logger.LogInformation("Decompressing retrieved document content");

        var decompressedContent = DecompressGzip(encodedAndCompressedContent);
        document.Content = _htmlSanitizer.GetBodyContent(decompressedContent);

      }

      return document;
    }

    private static string DecompressGzip(string str)
    {
      byte[] gzipBuffer = Convert.FromBase64String(str);

      using (MemoryStream memoryStream = new MemoryStream())
      {
        int msgLength = BitConverter.ToInt32(gzipBuffer, 0);
        memoryStream.Write(gzipBuffer, 0, gzipBuffer.Length);

        byte[] buffer = new byte[msgLength];

        memoryStream.Position = 0;
        int length;
        using (GZipStream zip = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
          length = zip.Read(buffer, 0, buffer.Length);
        }

        var data = new byte[length];
        Array.Copy(buffer, data, length);
        return Encoding.UTF8.GetString(data);

      }
    }
  }
}