using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    internal interface IMetricData
    {
        IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs();
    }
}