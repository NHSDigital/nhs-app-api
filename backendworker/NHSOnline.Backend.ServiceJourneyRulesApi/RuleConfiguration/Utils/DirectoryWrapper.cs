using System.IO;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class DirectoryWrapper : IDirectory
    {
        public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);
        
        public string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
            => Directory.GetFiles(path, searchPattern, enumerationOptions);
    }
}