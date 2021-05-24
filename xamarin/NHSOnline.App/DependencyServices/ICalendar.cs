using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.DependencyServices
{
    public interface ICalendar
    {
        void AddToCalendar(AddEventToCalendarRequest request);
        void ShowAlertPopup();
        Task<bool> RequestPermission();
    }
}