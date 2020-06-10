using System.Collections.Generic;
using System.Net;
using AutoFixture;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    public class EmisTestHelpers
    {
        private readonly IFixture _fixture;
        public EmisTestHelpers(IFixture fixture)
        {
            _fixture = fixture;
        }

        public EmisApiObjectResponse<T> CreateResponse<T>(
            HttpStatusCode statusCode,
            int? errorCode,
            string message = "")
        {
            var response = _fixture.Create<EmisApiObjectResponse<T>>();

            response.StatusCode = statusCode;
            response.StandardErrorResponse = CreateStandardErrorResponse(errorCode, message);
            response.ExceptionErrorResponse = CreateExceptionErrorResponse(errorCode, message);
            return response;
        }

        private static StandardErrorResponse CreateStandardErrorResponse(int? errorCode, string message)
        {
            return errorCode == null ? null : new StandardErrorResponse
            {
                InternalResponseCode = errorCode.Value,
                Message = message
            };
        }

        private static ExceptionErrorResponse CreateExceptionErrorResponse(int? errorCode, string message)
        {
            return new ExceptionErrorResponse
            {
                InternalResponseCode = errorCode,
                Exceptions = new List<ErrorResponseExceptionModel>
                {
                    new ErrorResponseExceptionModel
                    {
                        Message = message,
                    }
                }
            };
        }
    }
}
