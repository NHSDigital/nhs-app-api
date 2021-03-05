using System.Net;
using System.Threading.Tasks;

namespace NHSOnline.App.Areas.Cookies
{
    public interface ICookieView
    {
        Task AddCookie(Cookie cookie);
    }
}