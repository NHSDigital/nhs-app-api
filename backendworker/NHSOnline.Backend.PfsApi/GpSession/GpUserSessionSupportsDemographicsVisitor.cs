using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpUserSessionSupportsDemographicsVisitor : IGpUserSessionVisitor<bool>
    {
        public bool Visit(NullGpSession nullGpSession)
        {
            return false;
        }

        public bool Visit(GpUserSession gpSession)
        {
            return true;
        }
    }
}