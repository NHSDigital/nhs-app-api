using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Settings
{
    public class SpineLdapConfigurationSettings
    {
        public string LdapHost { get; }

        public int LdapPort { get; }

        public string LoginDN { get; }

        public string CertPath { get; }

        public string CertPassword { get; }

        public string NhsAppPartyId { get; }

        public SpineLdapConfigurationSettings() {}

        public SpineLdapConfigurationSettings(string ldapHost, int ldapPort, string loginDN, string certPath, string certPassword, string nhsAppPartyId)
        {
            LdapHost = ldapHost;
            LdapPort = ldapPort;
            LoginDN = loginDN;
            CertPath = certPath;
            CertPassword = certPassword;
            NhsAppPartyId = nhsAppPartyId;
        }

        public void Validate(bool ldapEnabled)
        {
            if (!ldapEnabled)
            {
                return;
            }

            if (LdapHost == null)
            {
                throw new ConfigurationNotFoundException(nameof(LdapHost));
            }

            if (LdapPort == default)
            {
                throw new ConfigurationNotFoundException(nameof(LdapPort));
            }

            if (LoginDN == null)
            {
                throw new ConfigurationNotFoundException(nameof(LoginDN));
            }

            if (CertPath == null)
            {
                throw new ConfigurationNotFoundException(nameof(CertPath));
            }

            if (CertPassword == null)
            {
                throw new ConfigurationNotFoundException(nameof(CertPassword));
            }

            if (NhsAppPartyId == null)
            {
                throw new ConfigurationNotFoundException(nameof(NhsAppPartyId));
            }
        }

        public static SpineLdapConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            SpineLdapConfigurationSettings config;
            var isLdapEnabled = bool.TrueString.Equals(configuration.GetOrWarn("SPINE_LDAP_LOOKUP_ENABLED", logger), StringComparison.OrdinalIgnoreCase);

            if (isLdapEnabled)
            {
                var ldapHost = configuration.GetOrThrow("SPINE_LDAP_HOST", logger);
                var ldapPort = configuration.GetIntOrThrow("SPINE_LDAP_PORT", logger);
                var loginDn = configuration.GetOrThrow("SPINE_LDAP_LOGIN_DN", logger);
                var certPath = configuration.GetOrThrow("SPINE_CERT_PATH", logger);
                var certPassword = configuration.GetOrThrow("SPINE_CERT_PASSWORD", logger);
                var nhsAppPartyId = configuration.GetOrThrow("NHS_APP_PARTY_ID_FOR_SPINE", logger);

                config = new SpineLdapConfigurationSettings(ldapHost, ldapPort, loginDn, certPath, certPassword, nhsAppPartyId);
            }
            else
            {
                config = new SpineLdapConfigurationSettings();
            }

            config.Validate(isLdapEnabled);

            return config;
        }
    }
}
