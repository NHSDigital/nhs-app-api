using System;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public interface IPrescriptionRequestValidationService
    {
        bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model);

        bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);
    }
}
