using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public abstract partial class ErrorTypes
    {
        private static List<ErrorTypes> DefinedTypes { get; } = LoadErrorTypes();

        public abstract string Prefix { get; }
        
        public abstract ErrorCategory Category { get; }

        public abstract int StatusCode { get; }

        public virtual SourceApi SourceApi => SourceApi.None;

        public static ErrorTypes LookupErrorType(ILogger logger, ErrorCategory category, int statusCode, SourceApi sourceApi = SourceApi.None)
        {
            try
            {
                return DefinedTypes.Single(x =>
                    x.Category == category &&
                    x.StatusCode == statusCode &&
                    (x.SourceApi == sourceApi || x.SourceApi == SourceApi.None));
            }
            catch (InvalidOperationException)
            {
                logger.LogWarning("Cannot find a single ErrorType matching the provided parameters: " +
                                   $"Category: {category}. " +
                                   $"StatusCode: {statusCode}. " +
                                   $"SourceApi: {sourceApi}.");
                return new UnhandledError();

            }
        }

        private static List<ErrorTypes> LoadErrorTypes()
        {
            return Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ErrorTypes).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass)
                .Select(t => (ErrorTypes)Activator.CreateInstance(t))
                .ToList();
        }

        public sealed class UnhandledError : ErrorTypes
        {
            public override string Prefix => "xx";
            public override ErrorCategory Category => ErrorCategory.None;
            public override int StatusCode => StatusCodes.Status500InternalServerError;
        }
    }
}