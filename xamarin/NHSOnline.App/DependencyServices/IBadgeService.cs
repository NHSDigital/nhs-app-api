using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices
{
    public interface IBadgeService
    {
        Task SetBadgeCount(string count);
    }
}