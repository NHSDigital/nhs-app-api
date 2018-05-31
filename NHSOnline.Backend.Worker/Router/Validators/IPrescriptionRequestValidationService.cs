using System;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Router.Validators
{
    public interface IPrescriptionRequestValidationService
    {
        bool IsValidFromDate(DateTimeOffset? fromDate, DateTimeOffset defaultFromDate);

        bool IsValidRepeatPrescriptionRequest(RepeatPrescriptionRequest model);
    }
}
