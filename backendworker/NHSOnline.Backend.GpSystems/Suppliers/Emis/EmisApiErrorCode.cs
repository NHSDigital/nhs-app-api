using Microsoft.Extensions.Hosting;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public enum EmisApiErrorCode
    {
        //Appointments
        None = 0,
        RequiredFieldValueMissing = -1014,
        ProvidedAppointmentSlotInPast = -1152,
        OnlineUserMaxAppointmentBookCount = -1156
    }
}
