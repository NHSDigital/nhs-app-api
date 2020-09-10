using System;
using System.Collections.Generic;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.Support.ResponseParsers
{
    public class XmlResponseParser : BaseResponseParser, IXmlResponseParser
    {
        private readonly Dictionary<string, string> _patternAndRedactors =
            new Dictionary<string, string>
            {
                { @"(There is an error in XML document)([\s\S]+)*", "$1" },
                { @"(The [a-z|A-Z]* ')[\s\S]+(' is not a valid [a-z|A-Z]* value.)", "$1***$2" },
                { @"[\s\S]+(was not expected.)", "There is an error in XML document" },
                { @"(There was an error reflecting property '[\S]+'.)", "$1" }
            };

        private readonly RedexPatternRedactor _redactor;

        public XmlResponseParser()
        {
            _redactor = new RedexPatternRedactor(_patternAndRedactors);
        }

        public override T ParseBody<T>(string stringResponse)
        {
            try
            {
                return stringResponse.DeserializeXml<T>();
            }
            catch (InvalidOperationException exception)
            {
                var redactedError = new[] { RedactExceptionMessage(exception) };
                throw new NhsUnparsableException("Response parsing failed.", redactedError);
            }
        }

        private NhsUnparsableExceptionError RedactExceptionMessage(InvalidOperationException exception)
        {
            var exceptionMessage = exception.InnerException?.Message ?? exception.Message;

            var redactedMessage = _redactor.RedactOrNull(exceptionMessage) ?? "Could not redact exception message";

            return new NhsUnparsableExceptionError(redactedMessage, "Unknown");
        }
    }
}
