using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public interface ITppPrescriptionMapper
    {
        PrescriptionListResponse Map(List<Medication> medications);  
    }
}