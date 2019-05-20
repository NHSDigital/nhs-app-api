using System.IO;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IFileHandler
    {
        string[] GetFiles(string directoryPath);

        bool ReadEmbeddedResourceFromFileName(string filePath, out FileData fileData);

        TextReader GetTextReader(string filePath);

        TextWriter GetTextWriter(string filePath);
    }
}