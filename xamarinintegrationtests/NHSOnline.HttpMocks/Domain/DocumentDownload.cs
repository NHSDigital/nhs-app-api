using Microsoft.AspNetCore.Http;

namespace NHSOnline.HttpMocks.Domain
{
    public class DocumentDownload
    {
        public string? Title { get; set; }

        public HttpRequest? Request { get; set; }

        public string? CorruptedBase64String { get; set; }

        public string? CorruptedPassKitBase64 { get; set; }

        public string? ImageBase64String { get; set; }

        public string? PkPassBase64String { get; set; }
    }
}