using System.Collections.Generic;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public interface IFileHandler
    {
        FileData ReadEmbeddedResourceFromFileName(string fileName);
        List<FileData> ReadContentFilesFromDirectory(string directory);
    }
}