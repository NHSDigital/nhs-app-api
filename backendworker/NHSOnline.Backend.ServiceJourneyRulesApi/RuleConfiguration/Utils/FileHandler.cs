using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace  NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class FileHandler : IFileHandler
    {
        private readonly ILogger _logger;
        private readonly Assembly _assembly;

        public FileHandler(ILogger<FileHandler> logger, Assembly assembly)
        {
            _logger = logger;
            _assembly = assembly;
        }

        public string[] GetFiles(string directoryPath)
            // ignore hidden and system files
            => Directory.GetFiles(directoryPath, Constants.Target.All, new EnumerationOptions());
        
        public bool ReadEmbeddedResourceFromFileName(string filePath, out FileData fileData)
        {
            try
            {
                var resourceName = _assembly.GetManifestResourceNames().Single(str => str.EndsWith(filePath, StringComparison.Ordinal));
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
        
        public TextReader GetTextReaderToReadFileContent(string filePath) => new StreamReader(filePath);
        
        private string GetEmbeddedText(string resourceName)
        {
            _logger.LogInformation($"Reading resource: {resourceName}");
            using (var resourceStream = _assembly.GetManifestResourceStream(resourceName))
            using (var stream = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return stream.ReadToEnd();
            }
        }
    }
}