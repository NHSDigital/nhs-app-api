using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
{
    public interface ISessionExtendBehaviour
    {
        Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel);
    }
}