using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public sealed class ErrorReferenceGenerator : IErrorReferenceGenerator
    {
        private readonly ILogger<ErrorReferenceGenerator> _logger;

        private readonly IRandomStringGenerator _randomStringGenerator;

        //string supplied should be lowercase and exclude b,d,i,l,p,q,v,0,1,2
        private const string CharactersForGenerator = "acefghjkmnorstuwxyz3456789";

        public ErrorReferenceGenerator(
            ILogger<ErrorReferenceGenerator> logger,
            IRandomStringGenerator randomStringGenerator)
        {
            _logger = logger;
            _randomStringGenerator = randomStringGenerator;
        }

        public string GenerateAndLogErrorReference(ErrorTypes errorTypes)
        {
            errorTypes ??= new ErrorTypes.UnhandledError();

            var reference = string.Concat(errorTypes.Prefix,
                _randomStringGenerator.GenerateString(4, CharactersForGenerator));

            _logger.LogInformation($"service_desk_error_reference={reference}");

            return reference;
        }

        public string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, SourceApi sourceApi)
        {
            var errorType = ErrorTypes.LookupErrorType(_logger, category, statusCode, sourceApi);
            return GenerateAndLogErrorReference(errorType);
        }

        public string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, Supplier supplier)
        {
            var sourceApi = SupplierSourceApiConverter.Instance[supplier];

            return GenerateAndLogErrorReference(category, statusCode, sourceApi);
        }
    }
}