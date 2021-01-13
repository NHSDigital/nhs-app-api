using System;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Prescriptions
{
    public class FakePrescriptionValidationService : IPrescriptionValidationService
    {
        public bool IsGetValid(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate)
        {
            return true;
        }

        public bool IsPostValid(RepeatPrescriptionRequest request, GpUserSession userSession)
        {
            return true;
        }

        public string MassageSpecialRequest(string specialRequest)
        {
            return specialRequest;
        }
    }
}