using System;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Prescriptions
{
    public interface IPrescriptionRequestValidationService
    {
        bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model);

        bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);
    }
}
