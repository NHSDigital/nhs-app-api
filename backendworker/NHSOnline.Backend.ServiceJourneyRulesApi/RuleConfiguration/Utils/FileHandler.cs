using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace  NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class FileHandler : IFileHandler
    {
        private readonly IErrorHandler _errorHandler;
        private readonly Assembly _assembly;

        public FileHandler(IErrorHandler errorHandler, Assembly assembly)
        {
            _errorHandler = errorHandler;
            _assembly = assembly;
        }

        public FileData ReadEmbeddedResourceFromFileName(string fileName)
        {
            try
            {
                var resourceName = _assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName, StringComparison.Ordinal));
                return ReadEmbeddedResource(resourceName);
            }
            catch (Exception e)
            {
                _errorHandler.LogError($"Error reading {fileName}. No or multiple resources found.");
                return new FileData
                {
                    Name = fileName,
                    IsError = true,
                    Error = $"{e}"
                };
            }
        }

        private FileData ReadEmbeddedResource(string resourceName)
        {
            try
            {
                _errorHandler.LogInformation($"Reading resource: {resourceName}");
                using (var resourceStream = _assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    return new FileData
                    {
                        Name = resourceName,
                        Data = reader.ReadToEnd()
                    };
                }
            }
            catch (Exception e)
            {
                _errorHandler.LogError($"Error reading resource: {resourceName}");
                return new FileData
                {
                    Name = resourceName,
                    IsError = true,
                    Error = $"{e}"
                };
            }
        }

        public List<FileData> ReadContentFilesFromDirectory(string directory)
        {
            var result = new List<FileData>();

            try
            {
                var directoryPath = $"{Path.GetDirectoryName(_assembly.Location)}/{directory}";
                var filesInDirectory = GetAllFilesInDirectory(directoryPath);

                foreach (var fileName in filesInDirectory)
                {
                    using (var fileReader = new StreamReader(fileName))
                    {
                        var fileData = fileReader.ReadToEnd();
                        result.Add(new FileData
                        {
                            Name = fileName,
                            Data = fileData
                        });
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _errorHandler.LogError($"Error reading files from directory: {directory}. {e.Message}");
                return new List<FileData>();
            }
        }

        private List<string> GetAllFilesInDirectory(string baseDirectory)
        {
            var results = new List<string>();

            results.AddRange(Directory.GetFiles(baseDirectory));
            
            foreach (var subDirectory in Directory.GetDirectories(baseDirectory))
            {
                results.AddRange(GetAllFilesInDirectory(subDirectory));
            }

            return results;
        }
    }
}