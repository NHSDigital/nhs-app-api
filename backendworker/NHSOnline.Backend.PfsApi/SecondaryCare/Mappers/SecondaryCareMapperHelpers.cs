using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Mappers
{
    public abstract class SecondaryCareMapperBase
    {
        public object GetValueFromExtensionWithUrl<T>(IList<Extension> extensions, string extensionLocator, ILogger logger, bool logError = true)
        {
            var extension = GetExtensionByUrl(extensions, extensionLocator, logger, false);

            if (extension?.Value is T value)
            {
                return value;
            }

            if (logError)
            {
                logger.LogError("Expected Extension value to be of type {T} but was {TypeName}",
                    typeof(T),
                    extension?.Value.GetType());
            }

            return null;
        }

        public Extension GetExtensionByUrl(IList<Extension> extensions, string extensionLocator, ILogger logger, bool logErrors = true)
        {
            try
            {
                return extensions.First(e => string.Equals(e.Url, extensionLocator, StringComparison.Ordinal));
            }
            catch (InvalidOperationException e)
            {
                if (logErrors)
                {
                    logger.LogError(e, "Could not find Extension of type {Url}", extensionLocator);
                }
            }

            return null;
        }
    }
}