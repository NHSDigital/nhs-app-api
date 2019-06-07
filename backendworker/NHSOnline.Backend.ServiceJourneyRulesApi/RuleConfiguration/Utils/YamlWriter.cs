namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class YamlWriter : IYamlWriter
    {
        private readonly IFileHandler _fileHandler;
        private readonly IYamlSerializer _serializer;

        public YamlWriter(IFileHandler fileHandler, IYamlSerializer serializer)
        {
            _fileHandler = fileHandler;
            _serializer = serializer;
        }

        public void Write<TModel>(string filePath, TModel model)
            where TModel : class, new()
        {
            using (var writer = _fileHandler.GetTextWriter(filePath))
            {
                _serializer.Serialize(writer, model);
            }
        }
    }
}