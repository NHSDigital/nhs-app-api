using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage
{
    public sealed class FileStorageWrapper : ISourceWrapper
    {
        private CloudBlobContainer _container;

        internal TestLogs Logs { get; set; }

        async Task ISourceWrapper.Initialise(TestLogs logs)
        {
            Logs = logs;
            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=http;" +
                "AccountName=devstoreaccount1;" +
                "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
                "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;" +
                "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;");

            var containerName = "integration-tests";
            var myClient = storageAccount.CreateCloudBlobClient();
            _container = myClient.GetContainerReference(containerName);
            _container.CreateIfNotExists();
            ClearContainer();
            await Task.CompletedTask;
        }

        private void ClearContainer()
        {
            var contents = _container.ListBlobs().OfType<CloudBlockBlob>().ToList();
            foreach (var content in contents)
            {
                content.Delete();
            }
        }

        internal void AssertContainerIsEmpty()
        {
            var itemsInContainer = _container.ListBlobs();
            itemsInContainer.Where(x => x.GetType() == typeof(CloudBlockBlob)).Should().BeEmpty();
        }

        internal void DeleteContainer()
        {
            ClearContainer();
            _container.Delete();
        }

        internal IList<CloudBlockBlob> ItemsInDirectory(string directoryName)
        {
            var directory = _container.GetDirectoryReference(directoryName);
            var list = directory.ListBlobs();
            if (list.Any())
            {
                var blobNames = list.OfType<CloudBlockBlob>().ToList();
                return blobNames;
            }

            return new List<CloudBlockBlob>();
        }
    }
}