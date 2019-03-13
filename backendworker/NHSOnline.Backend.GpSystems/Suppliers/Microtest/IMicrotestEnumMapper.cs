using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public interface IMicrotestEnumMapper
    {
        Channel MapChannel(string microtestChannel, Channel? defaultValue);
    }
}