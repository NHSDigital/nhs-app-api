using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Session.Models;

namespace NHSOnline.Backend.Worker.Router.Session
{
    public interface ISessionService
    {
        Task<SessionCreateResult> Create(UserSessionRequest request);
    }
}
