using System.Threading.Tasks;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services
{
    public class NhsAppCookieService
    {
        private readonly ICookieService _cookieService;
        private readonly IConfiguration _configuration;
        private const string GettingStartedCookieName = "SkipPreRegistrationPage";

        public NhsAppCookieService(ICookieService cookieService, IConfiguration configuration)
        {
            _cookieService = cookieService;
            _configuration = configuration;
        }

        public async Task<bool> HasGettingStartedPageCookie()
        {
            var cookie = await _cookieService.GetCookie(_configuration.NhsAppWeb.BaseAddress, GettingStartedCookieName).ResumeOnThreadPool();
            return cookie != null;
        }
    }
}