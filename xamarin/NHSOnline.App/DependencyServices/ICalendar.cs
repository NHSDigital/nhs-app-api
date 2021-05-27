using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface ICalendar
    {
        void AddToCalendar(AddEventToCalendarRequest request);
        void ShowCalendarPermissionDeniedAlert();
        void ShowCalendarAlertWhenValidationFails();
        Task<bool> RequestPermission();
    }
}