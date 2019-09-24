using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications.Azure
{
    public class AppleRegistrationDescriptionTestWrapper : AppleRegistrationDescription
    {
        public new string RegistrationId
        {
            get => base.RegistrationId;
            set => SetPrivateBaseTypePropertyValue(this, nameof(RegistrationId), value);
        }

        public new string DeviceToken
        {
            get => base.DeviceToken;
            set => SetPrivateBaseTypePropertyValue(this, nameof(DeviceToken), value);
        }

        public new DateTime? ExpirationTime
        {
            get => base.ExpirationTime;
            set => SetPrivateBaseTypePropertyValue(this, nameof(ExpirationTime), value);
        }

        public new ISet<string> Tags
        {
            get => base.Tags;
            set => SetPrivateBaseTypePropertyValue(this, nameof(Tags), value);
        }

        private static Dictionary<Type, Dictionary<string, PropertyInfo>> PropertiesCache { get; }

        static AppleRegistrationDescriptionTestWrapper()
        {
            PropertiesCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            GetProperties<AppleRegistrationDescription>();
        }

        public AppleRegistrationDescriptionTestWrapper() : base("abcdef")
        {
        }

        private static void GetProperties<T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var objectProperties = properties.ToDictionary(property => property.Name);

            PropertiesCache.TryAdd(type, objectProperties);
        }

        private static void SetPrivateBaseTypePropertyValue<T>(object instance, string propName, T val)
        {
            var type = instance.GetType().BaseType;
            if (PropertiesCache.TryGetValue(type, out var propertiesDictionary))
            {
                if (propertiesDictionary.TryGetValue(propName, out var propertyInfo))
                {
                    propertyInfo.SetValue(instance, val, null);
                }
            }
        }
    }
}