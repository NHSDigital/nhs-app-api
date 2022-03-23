using System.Threading.Tasks;
using NHSOnline.HttpMocks.Download;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    public sealed class DownloadWrapper : ISourceWrapper
    {
        private readonly string _pathToMockedFile = "http://host.docker.internal:8080/mockDownloadFile/";

        private DownloadResponse DownloadResponse => Init.WebServer.DownloadResponse;

        Task ISourceWrapper.Initialise(TestLogs logs)
        {
            DownloadResponse.Reset();
            return Task.CompletedTask;
        }

        public string AddFileToDownload(DownloadFile file)
        {
            DownloadResponse.SetResponse(file);
            return _pathToMockedFile + file.FileName;
        }
    }
}