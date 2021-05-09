using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications.Migration
{
    public interface IMigrationService
    {
        public Task<MigrationResult> Migrate(MigrationRequest request);
    }
}
