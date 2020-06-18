using System;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Registrations
{
    public class CreateRegistrationResultVisitor : IRegistrationResultVisitor<RegisterDeviceResult>
    {
        public RegisterDeviceResult Visit(RegistrationResult.Success result)
        {
            throw new NotImplementedException();
        }

        public RegisterDeviceResult Visit(RegistrationResult.BadGateway result)
        {
            return new RegisterDeviceResult.BadGateway();
        }

        public RegisterDeviceResult Visit(RegistrationResult.InternalServerError result)
        {
            return new RegisterDeviceResult.InternalServerError();
        }
    }
}