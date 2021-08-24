using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface ICalendar
    {
        Task AddToCalendar(AddEventToCalendarRequest request);
        Task ShowCalendarPermissionDeniedAlert();
        Task ShowCalendarAlertWhenValidationFails();
        Task<bool> RequestPermission();
    }
}