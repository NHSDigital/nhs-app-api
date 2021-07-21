using System;

namespace NHSOnline.IntegrationTests.UI
{
    internal static class ExceptionExtensions
    {
        public static bool AnyMessageContains(this Exception exception, string testString)
        {
            var current = exception;
            do
            {
                if (current.Message.Contains(testString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                current = current.InnerException;
            } while (current != null);

            return false;
        }
    }
}