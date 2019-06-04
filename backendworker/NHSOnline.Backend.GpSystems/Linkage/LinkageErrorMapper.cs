using System;
using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public abstract class LinkageErrorMapper
    {

        protected abstract Im1ConnectionErrorCodes.Code UnknownError { get; }

        private readonly Dictionary<HttpStatusCode, Func<Im1ConnectionErrorCodes.Code, LinkageResult>>
            _unknownErrorMappings =
                new Dictionary<HttpStatusCode, Func<Im1ConnectionErrorCodes.Code, LinkageResult>>()
                {
                    { HttpStatusCode.BadRequest, error => new LinkageResult.BadRequest(error) },
                    { HttpStatusCode.Conflict, error => new LinkageResult.Conflict(error) },
                    { HttpStatusCode.NotFound, error => new LinkageResult.NotFound(error) },
                    { HttpStatusCode.Forbidden, error => new LinkageResult.Forbidden(error) }
                };


        public LinkageResult MapUnknownError(HttpStatusCode statusCode)
        {
            var successfulMapping = _unknownErrorMappings.TryGetValue(statusCode, out var result);
            return successfulMapping ? result(UnknownError) : new LinkageResult.UnknownError(UnknownError);
        }
    }
}