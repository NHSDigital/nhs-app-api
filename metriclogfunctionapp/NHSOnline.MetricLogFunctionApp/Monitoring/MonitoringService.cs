using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Monitoring.AlertFormatters;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;
using NHSOnline.MetricLogFunctionApp.Slack;

namespace NHSOnline.MetricLogFunctionApp.Monitoring
{
    public sealed class MonitoringService
    {
        private readonly MonitoringConfig _config;
        private readonly SearchResultsMapper _searchResultsMapper;
        private readonly IEnumerable<IAlertFormatter> _formatters;
        private readonly AlertsMessageBuilderService _messageBuilderService;
        private readonly ISlackClient _slackClient;

        public MonitoringService(
            MonitoringConfig config,
            SearchResultsMapper searchResultsMapper,
            IEnumerable<IAlertFormatter> formatters,
            AlertsMessageBuilderService messageBuilderService,
            ISlackClient slackClient)
        {
            _config = config;
            _searchResultsMapper = searchResultsMapper;
            _formatters = formatters;
            _messageBuilderService = messageBuilderService;
            _slackClient = slackClient;
        }

        public async Task PostAlert(AppInsightAlert appInsightAlert)
        {
            var table = appInsightAlert.Data.AlertContext.SearchResults.Tables.First();
            var alertRule = appInsightAlert.Data.Essentials.AlertRule;

            var rows = _searchResultsMapper.MapSearchResults(table);

            var messageContent = CreateMessageContent(appInsightAlert, rows);
            var slackAlert = _messageBuilderService.BuildAlert(alertRule, messageContent);

            await _slackClient.PostMessage(slackAlert, _config.SlackChannel);
        }

        private List<string> CreateMessageContent(AppInsightAlert alert, List<Dictionary<string, object>> rows)
        {
            var formatter = FindFormatter(alert);
            return rows.Select(row => formatter.CreateMessageRow(row)).ToList();
        }

        private IAlertFormatter FindFormatter(AppInsightAlert appInsightAlert)
        {
            foreach (var formatter in _formatters)
            {
                if (formatter.CanFormat(appInsightAlert))
                {
                    return formatter;
                }
            }
            return new UnknownAlertFormatter();
        }
    }
}