using AutoFixture;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests
{
    internal static class MessageHelpers
    {
        internal static UserMessage MockUserMessage(IFixture fixture)
        {
            //Cannot use a fixture create on user message because of the auto generation of Id
            return fixture.Build<UserMessage>()
                .Without(x => x.Id)
                .Create();
        }
    }
}
