using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using static NHSOnline.Backend.Support.Constants.UsersConstants;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    internal class RegistrationDescriptionFactory : IRegistrationDescriptionFactory
    {
        public RegistrationDescription Create(RegisterDeviceRequest registerDeviceRequest, string nhsLoginId)
        {
            if (string.IsNullOrWhiteSpace(nhsLoginId))
            {
                throw new ArgumentException("NhsLoginId is null", nameof(registerDeviceRequest));
            }
            
            var tags = new HashSet<string> { $"{NhsLoginIdTagPrefix}{TagSeparator}{nhsLoginId}" };
            
            RegistrationDescription registrationDescription;

            switch (registerDeviceRequest.DeviceType)
            {
                case DeviceType.Android:
                    registrationDescription = new FcmRegistrationDescription(registerDeviceRequest.DevicePns, tags);
                    break;
                case DeviceType.Ios:
                    registrationDescription = new AppleRegistrationDescription(registerDeviceRequest.DevicePns, tags);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registerDeviceRequest));
            } 

            return registrationDescription;
        }
    }
}