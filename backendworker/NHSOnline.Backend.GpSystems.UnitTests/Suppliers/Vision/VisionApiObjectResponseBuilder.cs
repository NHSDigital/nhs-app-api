using System.Net;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    public static class VisionApiObjectResponseBuilder
    {
        public static VisionPfsApiObjectResponse<T> BuildUnsuccessfulResponseWithErrorCode<T>(string errorCode)
        {
            return new VisionPfsApiObjectResponse<T>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<T>
                {
                    Body = new VisionResponseBody<T>
                    {
                        VisionResponse = new VisionResponse<T>
                        {
                            ServiceHeader = new ServiceHeaderResponse
                            {
                                Outcome = new Outcome
                                {
                                    Successful = "false",
                                    Error = new OutcomeError
                                    {
                                        Code = errorCode
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}