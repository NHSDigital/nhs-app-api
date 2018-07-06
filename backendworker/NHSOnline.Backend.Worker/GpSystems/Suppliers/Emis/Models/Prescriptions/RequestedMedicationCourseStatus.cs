namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    public enum RequestedMedicationCourseStatus
    {
        Issued,
        Requested,
        ForwardedForSigning,
        Rejected,
        Unknown,
        Cancelled,
    }
}
