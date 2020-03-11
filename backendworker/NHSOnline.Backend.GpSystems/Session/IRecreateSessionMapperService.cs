using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Session
{
    public interface IRecreateSessionMapperService
    {
        GpUserSession Map(GpUserSession gpUserSession, string suid, string patientId);
    }
}