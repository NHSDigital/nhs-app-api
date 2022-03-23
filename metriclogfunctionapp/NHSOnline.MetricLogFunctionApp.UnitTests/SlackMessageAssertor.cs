using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.UnitTests
{
    public class SlackMessageAssertor
    {
        private readonly List<IExpectedMessageBlock> _expected = new List<IExpectedMessageBlock>();

        public SlackMessageAssertor TextBlock(string expectedContent)
        {
            _expected.Add(new ExpectedTextBlock(expectedContent));
            return this;
        }

        public SlackMessageAssertor DividerBlock()
        {
            _expected.Add(new ExpectedDividerBlock());
            return this;
        }

        public void Assert(IList<IBlock> actualMessage)
        {
            actualMessage.Count.Should().Be(_expected.Count);
            var actualAndExpectedMessage = actualMessage.Zip(_expected);
            actualAndExpectedMessage.ForEach(x=> x.Second.Assert(x.First));
        }

        private interface IExpectedMessageBlock
        {
            public void Assert(IBlock actualBlock);
        }

        private class ExpectedTextBlock : IExpectedMessageBlock
        {
            private readonly string _expectedContent;

            public ExpectedTextBlock(string expectedContent)
            {
                _expectedContent = expectedContent;
            }

            public void Assert(IBlock actualBlock)
            {
                actualBlock.Should().BeAssignableTo<SectionBlock>().Subject.text.text.Should()
                    .BeEquivalentTo(_expectedContent);
            }
        }

        private class ExpectedDividerBlock : IExpectedMessageBlock
        {
            public void Assert(IBlock actualBlock)
            {
                actualBlock.Should().BeAssignableTo<DividerBlock>();
            }
        }
    }
}