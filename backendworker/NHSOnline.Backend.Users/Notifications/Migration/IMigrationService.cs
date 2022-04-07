using System.Threading.Tasks;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Notifications.Migration
{
    public interface IMigrationService
    {
        public Task<MigrationResult> Migrate(MigrationRequest request);
    }
}
