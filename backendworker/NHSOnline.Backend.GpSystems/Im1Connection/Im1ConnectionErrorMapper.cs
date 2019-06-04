using System;
using System.Collections.Generic;
using System.Net;

namespace NHSOnline.Backend.GpSystems.Im1Connection
{
    public abstract class Im1ConnectionErrorMapper{

        protected abstract Im1ConnectionErrorCodes.Code UnknownError { get; }

        private readonly Dictionary<HttpStatusCode, Func<Im1ConnectionErrorCodes.Code, Im1ConnectionRegisterResult>>
            _unknownErrorMappings =
                new Dictionary<HttpStatusCode, Func<Im1ConnectionErrorCodes.Code, Im1ConnectionRegisterResult>>()
                {
                    { HttpStatusCode.BadRequest, error => new Im1ConnectionRegisterResult.BadRequest(error) },
                    { HttpStatusCode.NotFound, error => new Im1ConnectionRegisterResult.NotFound(error) },
                    { HttpStatusCode.Conflict, error => new Im1ConnectionRegisterResult.Conflict(error) },
                };

        protected Im1ConnectionRegisterResult MapUnknownError(HttpStatusCode statusCode)
        {
            var successfulMapping = _unknownErrorMappings.TryGetValue(statusCode, out var result);
            return successfulMapping
                ? result(UnknownError)
                : new Im1ConnectionRegisterResult.UnknownError(UnknownError);
        }
    }
}