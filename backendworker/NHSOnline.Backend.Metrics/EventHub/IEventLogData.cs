using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public interface IEventLogData
    {
        IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed);
    }
}