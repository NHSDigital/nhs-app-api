using System;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users.Registrations
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