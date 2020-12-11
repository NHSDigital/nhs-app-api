using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class UserSessionSerialiserService
    {
        private readonly JsonSerializerSettings _settings;

        public UserSessionSerialiserService()
        {
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = new RenameUserSessionSerializationBinder(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }

        internal string Serialise(UserSession userSession)
        {
            return JsonConvert.SerializeObject(userSession, _settings);
        }

        internal UserSession Deserialise(string sessionJson)
        {
            return JsonConvert.DeserializeObject<UserSession>(sessionJson, _settings);
        }

        private sealed class RenameUserSessionSerializationBinder : ISerializationBinder
        {
            private static readonly string P5UserSessionAssemblyName = typeof(P5UserSession).Assembly.FullName;
            private static readonly string P9UserSessionAssemblyName = typeof(P9UserSession).Assembly.FullName;

            private readonly DefaultSerializationBinder _defaultBinder = new DefaultSerializationBinder();

            public Type BindToType(string assemblyName, string typeName)
                => typeName switch
                {
                    nameof(P9UserSession) => typeof(P9UserSession),
                    nameof(P5UserSession) => typeof(P5UserSession),
                    _ => _defaultBinder.BindToType(assemblyName, typeName)
                };

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (serializedType == typeof(P5UserSession))
                {
                    assemblyName = P5UserSessionAssemblyName;
                    typeName = nameof(P5UserSession);
                }
                else if (serializedType == typeof(P9UserSession))
                {
                    assemblyName = P9UserSessionAssemblyName;
                    typeName = nameof(P9UserSession);
                }
                else
                {
                    _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);
                }
            }
        }
    }
}