using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Session
{
    public interface ISessionExtendService
    {
        Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel);
    }
}