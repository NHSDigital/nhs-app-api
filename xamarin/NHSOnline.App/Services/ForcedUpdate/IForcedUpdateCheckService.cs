using System.Threading.Tasks;

namespace NHSOnline.App.Services.ForcedUpdate
{
    public interface IForcedUpdateCheckService
    {
        public void Initiate();

        public Task<UpdateRequired> RequiresForcedUpdate();
    }
}