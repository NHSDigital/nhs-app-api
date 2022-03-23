using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Download
{
    public sealed class DownloadResponse
    {
        private readonly IDictionary<string, DownloadFile> _availableFiles = new Dictionary<string, DownloadFile>();

        public void SetResponse(DownloadFile file)
        {
            _availableFiles.Add(file.FileName, file);
        }

        public DownloadFile GetResponse(string filename)
        {
            return _availableFiles.ContainsKey(filename) ? _availableFiles[filename] : null;
        }

        public void Reset()
        {
            _availableFiles.Clear();
        }
    }
}