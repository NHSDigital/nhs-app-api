namespace NHSOnline.HttpMocks.Download
{
    public sealed class DownloadFile
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}