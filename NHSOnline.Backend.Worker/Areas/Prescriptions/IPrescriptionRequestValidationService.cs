using System;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public interface IPrescriptionRequestValidationService
    {
        bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);

        bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model);
    }
}
