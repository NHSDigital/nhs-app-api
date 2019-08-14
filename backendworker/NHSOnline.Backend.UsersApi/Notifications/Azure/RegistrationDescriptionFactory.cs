using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using static NHSOnline.Backend.Support.Constants.UsersConstants;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    public class RegistrationDescriptionFactory : IRegistrationDescriptionFactory
    {
        
        public RegistrationDescription Create(NotificationRegistrationRequest notificationRegistrationRequest)
        {
            if (string.IsNullOrWhiteSpace(notificationRegistrationRequest.NhsLoginId))
            {
                throw new ArgumentException("NhsLoginId is null", nameof(notificationRegistrationRequest));
            }
            
            var tags = new HashSet<string> { $"{NhsLoginIdTagPrefix}{TagSeparator}{notificationRegistrationRequest.NhsLoginId}" };
            
            RegistrationDescription registrationDescription;

            switch (notificationRegistrationRequest.DeviceType)
            {
                case DeviceType.Android:
                    registrationDescription = new FcmRegistrationDescription(notificationRegistrationRequest.DevicePns, tags);
                    break;
                case DeviceType.Ios:
                    registrationDescription = new AppleRegistrationDescription(notificationRegistrationRequest.DevicePns, tags);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationRegistrationRequest));
            } 

            return registrationDescription;
        }
    }
}