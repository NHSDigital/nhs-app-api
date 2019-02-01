using System.Collections.Generic;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public interface ITppPrescriptionMapper
    {
        PrescriptionListResponse Map(List<Medication> medications);  
    }
}