using System.Collections.Generic;
using System.Linq;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;

namespace NHSOnline.MetricLogFunctionApp.Monitoring
{
    public class SearchResultsMapper
    {
        public List<Dictionary<string, object>> MapSearchResults(AppInsightsTable table)
        {
            var columnDictionary = table.Columns.Select((x, y) => new {index = y, name = x.Name}).ToDictionary(x => x.index, x => x.name);
            var rowDictionary = table.Rows.Select(r => r.Select((x, y) => new {index = y, value = x})
                .ToDictionary(x => columnDictionary[x.index], x => x.value)).ToList();
            return rowDictionary;
        }
    }
}