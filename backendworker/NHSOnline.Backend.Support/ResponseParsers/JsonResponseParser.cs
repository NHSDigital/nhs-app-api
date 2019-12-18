using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.Support.ResponseParsers
{
    public class JsonResponseParser : BaseResponseParser, IJsonResponseParser
    {
        private readonly Dictionary<string, string> _patternAndRedactors =
            new Dictionary<string, string>()
            {
                { @"(Could not convert [a-z|A-Z]* to [a-z|A-Z]*):([\s\S]+)*", "$1" },
                { @"(Error converting value {null} to type [\s\S]+).Path([\s\S]+)*", "$1" },
                {
                    @"(Error converting value [\S]+ to type ([\s\S]+)).Path([\s\S]+)*",
                    "Error converting value to type $2"
                },
                { @"(After parsing a value an unexpected character was encountered):([\s\S]+)*", "$1" },
                { @"(Infinite loop detected from error handling.)([\s\S]+)*", "$1" },
                { @"(The reader's MaxDepth of ([\s\S]+) has been exceeded.)([\s\S]+)*", "$1" },
                { @"(Could not convert to [a-z|A-Z]*):([\s\S]+)*", "$1" },
                { @"(Error reading [a-z|A-Z]*. Unexpected token):([\s\S]+)*", "$1" },
                { @"(Unexpected end when reading bytes)([\s\S]+)*", "$1" },
                { @"(Unexpected token when reading bytes)([\s\S]+)*", "$1" },
                { @"(Unexpected end when reading JSON.)([\s\S]+)*", "$1" },
                { @"(Unexpected character encountered while parsing value)([\s\S]+)*", "$1" },
                {
                    @"(JsonToken [a-z|A-Z]* is not valid for closing JsonType [a-z|A-Z]*)([\s\S]+)*",
                    "JsonToken is not valid for closing JsonType"
                },
                {
                    @"(While setting the reader state back to current object an unexpected JsonType was encountered):([\s\S]+)*",
                    "$1"
                },
                { @"(Not a valid close JsonToken):([\s\S]+)*", "$1" },
                { @"(An undefined token is not a valid [a-z|A-Z]*).([\s\S]+)*", "$1" },
                { @"(Invalid character after parsing property name.)([\s\S]+)*", "$1" }
            };

        private readonly RedexPatternRedactor _redactor;

        public JsonResponseParser()
        {
            _redactor = new RedexPatternRedactor(_patternAndRedactors);
        }

        public override T ParseBody<T>(string stringResponse)
        {
            try
            {
                var serializedResponse = Deserialize<T>(stringResponse);

                if (serializedResponse != null)
                {
                    return  serializedResponse;
                }

                return default;
            }
            catch (JsonException exception)
            {
                throw new NhsUnparsableException("Response parsing failed.", exception);
            }
        }

        private T Deserialize<T>(string stringResponse)
        {
            var errors = new List<ErrorContext>();

            var response = JsonConvert.DeserializeObject<T>(stringResponse,
                new JsonSerializerSettings
                {
                    Error = delegate(object sender, ErrorEventArgs args)
                    {
                        errors.Add(args.ErrorContext);
                        args.ErrorContext.Handled = true;
                    }
                });

            if (errors.Any())
            {
                var redactedErrors = errors.Select(RedactExceptionMessage);
                throw new NhsUnparsableException("Response parsing failed.", redactedErrors);
            }

            return response;
        }

        public NhsUnparsableExceptionError RedactExceptionMessage(ErrorContext exception)
        {
            var exceptionMessage = exception.Error.Message;

            var redactedMessage =
                _redactor.RedactOrNull(exceptionMessage)
                ?? "Could not redact exception message";

            return new NhsUnparsableExceptionError(redactedMessage, exception.Path);
        }
    }
}