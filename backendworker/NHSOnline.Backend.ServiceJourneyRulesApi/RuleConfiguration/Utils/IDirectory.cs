using System.IO;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IDirectory
    {
        void CreateDirectory(string directory);
        string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions);
    }
}