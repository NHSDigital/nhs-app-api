namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class AppointmentLocationMapper : BaseMapper<Emis.Models.Location, Areas.Appointments.Models.Location>
    {
        public override Areas.Appointments.Models.Location Map(Models.Location source)
        {
            return new Areas.Appointments.Models.Location
            {
                Id = source.LocationId.ToString(),
                DisplayName = source.LocationName
            };
        }
    }
}
