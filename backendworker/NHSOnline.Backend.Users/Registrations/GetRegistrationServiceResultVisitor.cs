using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users.Registrations
{
    public class GetRegistrationServiceResultVisitor : ISearchDeviceResultVisitor<RegistrationExistsResult>
    {
        public RegistrationExistsResult Visit(SearchDeviceResult.Found result)
        {
            return new RegistrationExistsResult.Found();
        }

        public RegistrationExistsResult Visit(SearchDeviceResult.NotFound result)
        {
            return new RegistrationExistsResult.NotFound();
        }

        public RegistrationExistsResult Visit(SearchDeviceResult.BadGateway result)
        {
            return new RegistrationExistsResult.BadGateway();
        }

        public RegistrationExistsResult Visit(SearchDeviceResult.InternalServerError result)
        {
            return new RegistrationExistsResult.InternalServerError();
        }
    }
}