using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class TppPrescriptionValidationService : PrescriptionValidationService
    {
        public TppPrescriptionValidationService(ILogger<TppPrescriptionValidationService> logger) : base(logger)
        {
        }

        protected override bool IsSupplierGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return true;
        }

        protected override bool IsSupplierPostValid(RepeatPrescriptionRequest request)
        {
            return true;
        }
    }
}
