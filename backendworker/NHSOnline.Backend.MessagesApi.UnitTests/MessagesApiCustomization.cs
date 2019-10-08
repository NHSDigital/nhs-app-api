using AutoFixture;
using MongoDB.Bson;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests
{
    internal class MessagesApiCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ObjectId());
            fixture.Register(() => new MessagesResponse
            {
                fixture.Create<SenderMessages>(),
                fixture.Create<SenderMessages>(),
                fixture.Create<SenderMessages>()
            });
        }
    }
}

