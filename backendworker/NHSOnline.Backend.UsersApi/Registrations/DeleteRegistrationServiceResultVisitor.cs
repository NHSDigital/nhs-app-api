using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Registrations
{
    public class DeleteRegistrationServiceResultVisitor : ISearchDeviceResultVisitor<DeleteRegistrationResult>,
        IDeleteDeviceResultVisitor<DeleteRegistrationResult>
    {
        public DeleteRegistrationResult Visit(SearchDeviceResult.Found result)
        {
            throw new System.NotImplementedException();
        }

        public DeleteRegistrationResult Visit(SearchDeviceResult.FoundMany result)
        {
            throw new System.NotImplementedException();
        }

        public DeleteRegistrationResult Visit(SearchDeviceResult.NotFound result)
        {
            return new DeleteRegistrationResult.NotFound();
        }

        public DeleteRegistrationResult Visit(SearchDeviceResult.BadGateway result)
        {
            return new DeleteRegistrationResult.BadGateway();
        }

        public DeleteRegistrationResult Visit(SearchDeviceResult.InternalServerError result)
        {
            return new DeleteRegistrationResult.InternalServerError();
        }

        public DeleteRegistrationResult Visit(DeleteDeviceResult.Success result)
        {
            return new DeleteRegistrationResult.Success();
        }

        public DeleteRegistrationResult Visit(DeleteDeviceResult.BadGateway result)
        {
            return new DeleteRegistrationResult.BadGateway();
        }

        public DeleteRegistrationResult Visit(DeleteDeviceResult.InternalServerError result)
        {
            return new DeleteRegistrationResult.InternalServerError();
        }
    }
}