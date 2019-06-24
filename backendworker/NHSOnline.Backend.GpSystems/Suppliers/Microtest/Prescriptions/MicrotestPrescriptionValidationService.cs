using System;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions
{
    public class MicrotestPrescriptionValidationService : PrescriptionValidationService
    {
        public MicrotestPrescriptionValidationService(ILogger<MicrotestPrescriptionValidationService> logger) : base(logger)
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
