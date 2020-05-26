using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public class DefaultSessionExtendBehaviour : ISessionExtendBehaviour
    {
        public async Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return await Task.FromResult<SessionExtendResult>(new SessionExtendResult.Success());
        }
    }
}