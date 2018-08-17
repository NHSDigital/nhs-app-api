using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public interface IEmisSessionService
    {
        Task<SessionsEndUserSessionPostResponse> SendSessionsEndUserSessionPost();
    }
}
