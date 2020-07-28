using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionReferenceCodeVisitor : IGpUserSessionVisitor<string>
    {
        public string Visit(NullGpSession nullGpSession)
        {
            return nullGpSession?.SessionCreateReferenceCode;
        }

        public string Visit(GpUserSession gpSession)
        {
            return null;
        }
    }
}