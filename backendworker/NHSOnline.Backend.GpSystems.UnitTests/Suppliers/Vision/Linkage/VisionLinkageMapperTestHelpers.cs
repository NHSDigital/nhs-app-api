using System.Net;
using AutoFixture;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    public class VisionLinkageMapperTestHelpers
    {
        private readonly IFixture _fixture;
        public VisionLinkageMapperTestHelpers(IFixture fixture)
        {
            _fixture = fixture;
        }

        public VisionLinkageApiObjectResponse<T> CreateResponse<T>(
            HttpStatusCode statusCode,
            string errorCode,
            string message = "")
        {
            var response = _fixture.Create<VisionLinkageApiObjectResponse<T>>();

            response.StatusCode = statusCode;
            response.ErrorResponse.Code = errorCode;
            response.ErrorResponse.Diagnostic = message;
            return response;
        }
    }
}