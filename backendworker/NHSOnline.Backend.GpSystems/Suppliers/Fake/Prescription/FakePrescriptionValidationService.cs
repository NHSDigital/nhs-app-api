using System;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class FakePrescriptionValidationService : IPrescriptionValidationService
    {
        public bool IsGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return true;
        }

        public bool IsPostValid(RepeatPrescriptionRequest request, int specialRequestCharacterLimit)
        {
            return true;
        }
    }
}