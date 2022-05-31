using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.Repository.UnitTests.SqlApi
{
    public class TestSqlApiRepositoryConfiguration : ISqlApiRepositoryConfiguration
    {
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public void Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}