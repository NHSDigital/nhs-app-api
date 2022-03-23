using System.Collections.Generic;
using System.Threading.Tasks;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.Slack
{
    public interface ISlackClient
    {
        Task PostMessage(IEnumerable<IBlock> blocks, string slackChannel);
    }
}