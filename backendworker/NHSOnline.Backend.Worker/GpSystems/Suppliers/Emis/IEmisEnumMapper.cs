using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public interface IEmisEnumMapper
    {
        Necessity MapNecessity(string emisNecessity, Necessity? defaultValue);
        
        Channel MapSlotTypeStatus(string emisSlotTypeStatus, Channel? defaultValue);
    }
}
