namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class Appointment
    {
        public int SlotId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int SessionId { get; set; }
        public string SlotTypeName { get; set; }
        public string SlotTypeStatus { get; set; }
        public TelephoneAppointmentDetails TelephoneAppointmentDetails { get; set; }
    }
}