using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace NHSOnline.IntegrationTestAnalyser
{
    public static class Downloader
    {
        private const string CollectionUri = "https://dev.azure.com/nhsapp";
        private const string ProjectName = "NHS%20App";
        private const string DefinitionName = "nhsapp CI";

        internal static async Task<string> DownloadBuildArtifacts(string personalAccessToken)
        {
            var downloadFolder = Guid.NewGuid().ToString();
            Console.WriteLine($"Creating Directory: \"{downloadFolder}\"");
            Directory.CreateDirectory(downloadFolder);

            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);

            using var connection = new VssConnection(new Uri(CollectionUri), credentials);
            using var buildClient = connection.GetClient<BuildHttpClient>();

            var builds = await GetBuilds(buildClient);

            foreach (var build in builds)
            {
                Console.WriteLine($"Looking for artifacts for build {build.Id}");
                await DownloadArtifactsForBuild(buildClient, build, downloadFolder, personalAccessToken);
            }

            return downloadFolder;
        }

        private static async Task DownloadArtifactsForBuild(BuildHttpClient buildClient, Build build,
            string downloadFolder, object personalAccessToken)
        {
            try
            {
                var artifacts = await buildClient.GetArtifactsAsync(ProjectName, build.Id);

                foreach (var currentArtifact in artifacts.FindAll(artifact =>
                    artifact.Name.StartsWith("Xamarin Integration Test Files", StringComparison.OrdinalIgnoreCase) ||
                    artifact.Name.StartsWith("Xamarin Integration Upgrade Test Files", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine($"Downloading artifact {currentArtifact.Name}");
                    var downloadUrl = currentArtifact.Resource.DownloadUrl;
                    if (downloadUrl != null)
                    {
                        await DownloadFile(downloadUrl, $"{currentArtifact.Name}-{build.LastChangedDate:yyyyMMdd-hhmmss}-{build.Id}.zip", downloadFolder, personalAccessToken);
                    }
                }
            }
            catch (ArtifactNotFoundException)
            {
                Console.WriteLine(
                    $"Artifact does not exist for this for build {build.Id}");
            }
        }

        private static async Task DownloadFile(string downloadUrl, string fileName, string downloadFolder,
            object personalAccessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes($":{personalAccessToken}")));

                var tempFile = Path.GetTempFileName();
                await using (var response = await client.GetStreamAsync(new Uri(downloadUrl)))
                {
                    Console.WriteLine($"{fileName}");

                    await using var fileStream = File.OpenWrite(tempFile);
                    await response.CopyToAsync(fileStream);
                }
                var fullPath = Path.Combine(downloadFolder, fileName);

                Console.WriteLine($"Extracting \"{fileName}\" ");
                ZipFile.ExtractToDirectory(tempFile, fullPath);

            }
        }

        private static async Task<List<Build>> GetBuilds(BuildHttpClient buildClient)
        {
            var definitions = await buildClient.GetDefinitionsAsync(ProjectName, DefinitionName);
            var definitionIds = definitions.Select(d => d.Id);

            return await buildClient.GetBuildsAsync(
                ProjectName, definitionIds, top: 100);
        }
    }
}