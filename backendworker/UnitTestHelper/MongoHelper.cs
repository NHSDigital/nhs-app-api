using System.Collections.Generic;
using System.Threading;
using AutoFixture;
using MongoDB.Driver;
using Moq;

namespace UnitTestHelper
{
    public static class MongoHelper
    {
        public static Mock<IAsyncCursor<T>> CreateCursorMockFindNone<T>(IFixture fixture)
        {
            var cursorMock = fixture.Create<Mock<IAsyncCursor<T>>>();
            cursorMock.Setup(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);
            cursorMock.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
            return cursorMock;
        }

        public static Mock<IAsyncCursor<T>> CreateCursorMockFind<T>(IFixture fixture, IEnumerable<T> values)
        {
            var cursorMock = fixture.Create<Mock<IAsyncCursor<T>>>();
            var mockReturn = true;

            cursorMock.Setup(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(() => mockReturn)
                .Callback<CancellationToken>(t => mockReturn = false);

            cursorMock.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => mockReturn)
                .Callback<CancellationToken>(t => mockReturn = false);

            cursorMock.SetupGet(x => x.Current).Returns(values);
            return cursorMock;
        }
    }
}