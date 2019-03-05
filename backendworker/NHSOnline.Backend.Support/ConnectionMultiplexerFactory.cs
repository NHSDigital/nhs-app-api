using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StackExchange.Redis;

namespace NHSOnline.Backend.Support
{
    public interface IConnectionMultiplexerFactory
    {
        IConnectionMultiplexer GetMultiplexer(ConnectionMultiplexerName name);
    }

    public class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        private readonly IDictionary<ConnectionMultiplexerName, IConnectionMultiplexer> _multiplexers;

        public ConnectionMultiplexerFactory(IEnumerable<NamedConnectionMultiplexer> multiplexers)
        {
            _multiplexers = multiplexers.ToDictionary(n => n.Name, n => n.Multiplexer);
        }

        public IConnectionMultiplexer GetMultiplexer(ConnectionMultiplexerName name)
        {
            if (!_multiplexers.TryGetValue(name, out var multiplexer))
            {
                throw new ArgumentOutOfRangeException(nameof(name),
                    string.Format(CultureInfo.InvariantCulture,
                ExceptionMessages.ConnectionMultiplexerFactoryUnknownMultiplexerName, name));
            }
            
            return multiplexer;
        }
    }

    public enum ConnectionMultiplexerName
    {
        OdsCodeLookup
    }

    public class NamedConnectionMultiplexer
    {
        public NamedConnectionMultiplexer(ConnectionMultiplexerName name, IConnectionMultiplexer multiplexer)
        {
            Name = name;
            Multiplexer = multiplexer;
        }

        public ConnectionMultiplexerName Name { get; }
        public IConnectionMultiplexer Multiplexer { get; }
    }
}
