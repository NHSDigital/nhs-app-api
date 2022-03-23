using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.Slack
{
    internal sealed class SlackClient : ISlackClient
    {
        private readonly ILogger<SlackClient> _logger;
        private readonly SlackTaskClient _client;

        public SlackClient(ILogger<SlackClient> logger, SlackConfig slackConfig)
        {
            _logger = logger;
            _client = new SlackTaskClient(slackConfig.Token);
        }

        public async Task PostMessage(IEnumerable<IBlock> blocks, string slackChannel)
        {
            try
            {
                var response = await _client.PostMessageAsync(slackChannel, string.Empty, blocks: blocks.ToArray());
                if (!response.ok)
                {
                    var slackMessageContent = string.Join("\r\n", blocks.Select(BlockToLogString));
                    _logger.LogError($"Failed to post to slack channel '{slackChannel}' because '{response.error}'. Attempted slack message: \r\n{slackMessageContent}");
                    throw new Exception(response.error);
                }

                _logger.LogInformation("Message to Slack response: {SlackResponse}", response.ok);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending message to Slack Channel {SlackChannel}: {Message}", slackChannel,
                    e.Message);
                throw;
            }
        }

        private string BlockToLogString(IBlock block)
        {
            return block switch
            {
                DividerBlock _ => "---",
                SectionBlock sectionBlock => sectionBlock.text.text,
                _ => throw new Exception($"Cannot parse slack block type {nameof(block)}")
            };
        }
    }
}