using System;

namespace Nhs.App.Api.Integration.Tests.Services
{
    public class AccessTokenGenerationException : Exception
    {
        public AccessTokenGenerationException()
        {
        }

        public AccessTokenGenerationException(string message)
            : base(message)
        {
        }

        public AccessTokenGenerationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
