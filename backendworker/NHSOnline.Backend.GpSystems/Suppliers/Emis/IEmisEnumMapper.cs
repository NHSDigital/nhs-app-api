using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public interface IEmisEnumMapper
    {
        Necessity MapNecessity(string emisNecessity, Necessity? defaultValue);
        
        Channel MapSlotTypeStatus(string emisSlotTypeStatus, Channel? defaultValue);
    }
}
