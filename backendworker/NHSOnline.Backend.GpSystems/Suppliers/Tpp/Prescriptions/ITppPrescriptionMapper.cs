using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    public interface ITppPrescriptionMapper
    {
        PrescriptionListResponse Map(List<Medication> medications);  
    }
}