using System.Threading.Tasks;
using NHSOnline.App.Controls.WebViews.Payloads;

namespace NHSOnline.App.Services.Media
{
    public interface ISelectMediaService
    {
        Task SelectMedia(ISelectMediaRequest selectMediaRequest);
    }
}