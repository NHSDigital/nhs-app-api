using System.Collections.Generic;
using System.Linq;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.Slack
{
    internal sealed class SlackMessageBuilder
    {
        private readonly IList<IBlock> _blocks = new List<IBlock>();

        public SlackMessageBuilder Add(string content)
        {
            var sectionBlock = new SectionBlock
            {
                text = new Text
                {
                    type = TextTypes.Markdown,
                    text = content
                }
            };
            _blocks.Add(sectionBlock);
            return this;
        }

        public SlackMessageBuilder AddIndented(params string[] content)
        {
            var text = string.Join("\r\n", content.ToList().Select(x=> $"> {x}"));
            return Add(text);
        }

        public SlackMessageBuilder AddDivider()
        {
            _blocks.Add(new DividerBlock());
            return this;
        }

        public IList<IBlock> Build()
        {
            return _blocks;
        }
    }
}