using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.IntegrationTestAnalyser
{
    internal class Program
    {
        private static async Task Main()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();

            var analyserConfiguration = config.GetSection("Analyser").Get<AnalyserConfiguration>();

            if (analyserConfiguration == null || string.IsNullOrEmpty(analyserConfiguration.DevOpsToken))
            {
                Console.WriteLine("Please set Dev Ops build token by running 'dotnet user-secrets set \"Analyser:DevOpsToken\" \"<read_only_token>'");
                return;
            }

            var buildArtifactDirectory = await Downloader.DownloadBuildArtifacts(analyserConfiguration.DevOpsToken);

            var trxFiles = Directory
                .EnumerateFiles(buildArtifactDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetExtension(s).Equals(".trx", StringComparison.Ordinal));

            var analyser = new Analyser(trxFiles);

            await analyser.Analyse();
        }
    }

}