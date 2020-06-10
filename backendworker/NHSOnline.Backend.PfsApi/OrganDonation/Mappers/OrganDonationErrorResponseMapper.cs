using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal abstract class OrganDonationErrorResponseMapper<T> : IMapper<HttpStatusCode, T>
        where T: class
    {      
        private readonly ILogger<T> _logger;
        
        protected readonly PfsErrorResponse RetryResponse = new PfsErrorResponse
        {
            ErrorCode = 1,
            ErrorMessage = "A recoverable exception has occurred processing the request"
        };

        protected readonly PfsErrorResponse NoRetryResponse = new PfsErrorResponse
        {
            ErrorCode = 0,
            ErrorMessage = "A non-recoverable exception has occurred processing the request"
        };

        protected OrganDonationErrorResponseMapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }

        public T Map(HttpStatusCode source)
        {   
            if (!MappingTable.TryGetValue(source, out var codeResult))
            {
                codeResult = DefaultResult;
            }

            _logger.LogDebug(codeResult.Item2);
            
            return codeResult.Item1;
        }
        
        protected abstract Tuple<T, string> DefaultResult { get; }
 
        protected abstract Dictionary<HttpStatusCode, Tuple<T, string>>  MappingTable { get; }
    }
}


