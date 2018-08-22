using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Ndop.Models;

namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopService
    {
        Task<GetNdopResult> GetJwtToken(string nhsNumber);
    }
}