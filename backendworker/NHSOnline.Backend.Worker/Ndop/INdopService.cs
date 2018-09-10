using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopService
    {
        Task<GetNdopResult> GetJwtToken(string nhsNumber);
    }
}