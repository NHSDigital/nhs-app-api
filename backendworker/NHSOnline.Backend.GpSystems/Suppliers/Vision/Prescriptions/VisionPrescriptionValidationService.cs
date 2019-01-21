using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions
{
    public class VisionPrescriptionValidationService : PrescriptionValidationService
    {
        public VisionPrescriptionValidationService(ILogger<VisionPrescriptionValidationService> logger) : base(logger)
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
