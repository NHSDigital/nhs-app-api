using System;
using AutoFixture;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    internal static class MessageHelpers
    {
        internal static UserMessage MockUserMessage(IFixture fixture)
        {
            //Cannot use a fixture create on user message because of the auto generation of Id
            return new UserMessage
            {
                Body = fixture.Create<string>(),
                NhsLoginId = fixture.Create<string>(),
                Sender = fixture.Create<string>(),
                SentTime = fixture.Create<DateTime>(),
                Version = 1
            };
        }
    }
}
