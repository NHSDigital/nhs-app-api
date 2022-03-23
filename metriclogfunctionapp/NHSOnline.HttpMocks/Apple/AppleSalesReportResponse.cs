using NHSOnline.HttpMocks.Download;

namespace NHSOnline.HttpMocks.Apple
{
    public sealed class AppleSalesReportResponse
    {
        private DownloadFile _appleDownloads = null;

        public void Add(DownloadFile content)
        {
            _appleDownloads = content;
        }

        public void Reset()
        {
            _appleDownloads = null;
        }

        public DownloadFile Content()
        {
            return _appleDownloads;
        }
    }
}