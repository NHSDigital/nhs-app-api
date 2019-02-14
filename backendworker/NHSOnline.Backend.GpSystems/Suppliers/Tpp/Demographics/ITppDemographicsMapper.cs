using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics
{
    public interface ITppDemographicsMapper
    {
        DemographicsResponse Map(PatientSelectedReply patientSelectedReply);
    }
}