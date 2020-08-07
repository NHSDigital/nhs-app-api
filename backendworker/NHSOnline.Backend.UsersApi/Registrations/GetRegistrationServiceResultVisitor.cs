using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Registrations
{
    public class GetRegistrationServiceResultVisitor : ISearchDeviceResultVisitor<RegistrationExistsResult>,
        IDeleteDeviceResultVisitor<RegistrationExistsResult>
    {
        public RegistrationExistsResult Visit(SearchDeviceResult.Found result)
        {
            return new RegistrationExistsResult.Found();
        }

        public RegistrationExistsResult Visit(SearchDeviceResult.FoundMany result)
        {
            return new RegistrationExistsResult.InternalServerError();
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

        public RegistrationExistsResult Visit(DeleteDeviceResult.Success result)
        {
            return new RegistrationExistsResult.Found();
        }

        public RegistrationExistsResult Visit(DeleteDeviceResult.BadGateway result)
        {
            return new RegistrationExistsResult.BadGateway();
        }

        public RegistrationExistsResult Visit(DeleteDeviceResult.InternalServerError result)
        {
            return new RegistrationExistsResult.InternalServerError();
        }
    }
}