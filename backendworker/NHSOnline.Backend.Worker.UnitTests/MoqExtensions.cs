using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests
{
    public static class MoqExtensions
    {
        public static Moq.Language.Flow.ISetup<ILogger<T>> SetupLogger<T>(this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel, string message, Exception exception)
        {
            return mockLogger.Setup(x => x.Log(
                 logLevel,
                 It.IsAny<EventId>(),
                 It.Is<FormattedLogValues>(flv => flv.ToString().Contains(message, StringComparison.InvariantCulture)),
                 exception,
                 It.IsAny<Func<object, Exception, string>>()));
        }
    }
}
