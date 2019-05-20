using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class FileHandler : IFileHandler
    {
        private readonly ILogger _logger;
        private readonly IDirectory _directory;
        private readonly Assembly _assembly;

        public FileHandler(ILogger<FileHandler> logger, IDirectory directory, Assembly assembly)
        {
            _logger = logger;
            _directory = directory;
            _assembly = assembly;
        }

        public string[] GetFiles(string directoryPath)
            // ignore hidden and system files
            => _directory.GetFiles(directoryPath, Constants.Target.All, new EnumerationOptions());

        public bool ReadEmbeddedResourceFromFileName(string filePath, out FileData fileData)
        {
            try
            {
                var resourceName = _assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(filePath, StringComparison.Ordinal));
                fileData = new FileData(filePath, GetEmbeddedText(resourceName));

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error reading {filePath}. No or multiple resources found.");
                fileData = null;

                return false;
            }
        }

        public TextReader GetTextReader(string filePath) => new StreamReader(filePath);

        public TextWriter GetTextWriter(string filePath)
        {
            CreateDirectory(filePath);

            return new StreamWriter(filePath);
        }

        private string GetEmbeddedText(string resourceName)
        {
            _logger.LogInformation($"Reading resource: {resourceName}");
            using (var resourceStream = _assembly.GetManifestResourceStream(resourceName))
            using (var stream = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return stream.ReadToEnd();
            }
        }

        private void CreateDirectory(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            _directory.CreateDirectory(directory);
        }
    }
}