using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace UnitTestHelper
{
    public class ConfigurationStub : IConfiguration
    {
        private readonly Dictionary<string, string> _values;

        public ConfigurationStub(Dictionary<string, string> values)
        {
            _values = values;
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new System.NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new System.NotImplementedException();
        }

        public string this[string key]
        {
            get => _values[key];
            set => throw new System.NotImplementedException();
        }
    }
}