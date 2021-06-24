using System.Net;
using System.Threading.Tasks;

namespace NHSOnline.App.Controls.WebViews
{
    public interface ICookieService
    {
        Task SetCookie(Cookie cookie);
    }
}