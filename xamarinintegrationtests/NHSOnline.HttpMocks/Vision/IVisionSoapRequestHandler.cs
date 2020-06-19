using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Vision.Models;

namespace NHSOnline.HttpMocks.Vision
{
    public interface IVisionSoapRequestHandler
    {
        string Method { get; }

        VisionResponseEnvelope Handle(VisionPatient patient);
    }
}