using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support
{
    public enum ErrorCategory
    {
        None,
        Login,
        Timeout
    }
    
    //Disabling CA1717 (no plurals) as detects the i as a plural when not in this case
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification="False positive - Api is not a plural word")]
    public enum SourceApi
    {
        None = 0,
        Emis = 1,
        Tpp = 2,
        Vision = 3,
        Microtest = 4,
        NhsLogin = 5,
        ServiceJourneyRules = 6,
        OrganDonation = 7
    }

    public interface IErrorReferenceGenerator
    {
        string GenerateAndLogErrorReference(ErrorTypes errorTypes);
        
        string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, SourceApi sourceApi = SourceApi.None);
        
        string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, Supplier supplier);
    }
    
    public class ErrorReferenceGenerator : IErrorReferenceGenerator
    {
        private readonly ILogger<ErrorReferenceGenerator> _logger;
        private readonly IRandomStringGenerator _randomStringGenerator;
        //string supplied should be lowercase and exclude b,d,i,l,p,q,v,0,1,2
        private const string CharactersForGenerator = "acefghjkmnorstuwxyz3456789";

        private static readonly Dictionary<Supplier, SourceApi> SupplierToSourceApiMap =
            new Dictionary<Supplier, SourceApi>
            {
                { Supplier.Unknown, SourceApi.None },
                { Supplier.Emis, SourceApi.Emis },
                { Supplier.Tpp, SourceApi.Tpp },
                { Supplier.Vision, SourceApi.Vision },
                { Supplier.Microtest, SourceApi.Microtest }
            };

        private static List<ErrorTypes> ErrorTypes { get; }
        
        static ErrorReferenceGenerator()
        {
            ErrorTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ErrorTypes).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass)
                .Select(t => (ErrorTypes)Activator.CreateInstance(t))
                .ToList();
        }
        
        public ErrorReferenceGenerator(ILogger<ErrorReferenceGenerator> logger, IRandomStringGenerator randomStringGenerator)
        {
            _logger = logger;
            _randomStringGenerator = randomStringGenerator;
        }

        public string GenerateAndLogErrorReference(ErrorTypes errorTypes)
        {
            var prefix = errorTypes == null ? "xx" : errorTypes.Prefix;

            var reference = string.Concat(prefix, _randomStringGenerator.GenerateString(4, CharactersForGenerator));

            _logger.LogInformation($"service_desk_error_reference={reference}");

            return reference;
        }
        
        public string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, SourceApi sourceApi)
        {
            var errorType = LookupErrorType(category, statusCode, sourceApi);
            return GenerateAndLogErrorReference(errorType);
        }
        
        public string GenerateAndLogErrorReference(ErrorCategory category, int statusCode, Supplier supplier)
        {
            if (!SupplierToSourceApiMap.TryGetValue(supplier, out var sourceApi))
            {
                sourceApi = SourceApi.None;
            }

            return GenerateAndLogErrorReference(category, statusCode, sourceApi);
        }

        private static ErrorTypes LookupErrorType(ErrorCategory category, int statusCode, SourceApi sourceApi)
        {
            return ErrorTypes.SingleOrDefault(x =>
                x.Category == category &&
                x.StatusCode == statusCode &&
                x.SourceApi == sourceApi);
        }
    }
}

 