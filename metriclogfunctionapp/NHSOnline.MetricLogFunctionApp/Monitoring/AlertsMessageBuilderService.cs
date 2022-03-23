using System.Collections.Generic;
using System.Linq;
using NHSOnline.MetricLogFunctionApp.Slack;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.Monitoring
{
    public sealed class AlertsMessageBuilderService
    {
        public IEnumerable<IBlock> BuildAlert(string header, List<string> messageRows)
        {
            var slackBuilder = new SlackMessageBuilder();
            slackBuilder.Add(BuildSlackHeader(header));

            foreach (var message in messageRows)
            {
                slackBuilder.Add(message).AddDivider();
            }
            return slackBuilder.Build().ToArray();
        }

        private string BuildSlackHeader(string header)
        {
            return $":rotating_light:   *{header}*   :rotating_light:";
        }
    }
}