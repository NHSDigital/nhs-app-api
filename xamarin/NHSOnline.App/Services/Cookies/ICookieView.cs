using System.Net;
using System.Threading.Tasks;

namespace NHSOnline.App.Services.Cookies
{
    public interface ICookieView
    {
        Task AddCookie(Cookie cookie);
    }
}