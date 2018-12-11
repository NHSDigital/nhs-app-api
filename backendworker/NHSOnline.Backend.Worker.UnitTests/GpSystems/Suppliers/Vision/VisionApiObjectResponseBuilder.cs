using System.Net;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    public static class VisionApiObjectResponseBuilder
    {
        public static VisionPFSClient.VisionApiObjectResponse<T> BuildUnsuccessfulResponseWithErrorCode<T>(string errorCode)
        {
            return new VisionPFSClient.VisionApiObjectResponse<T>(HttpStatusCode.OK)
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
                                    Successful = bool.FalseString.ToLowerInvariant(),
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