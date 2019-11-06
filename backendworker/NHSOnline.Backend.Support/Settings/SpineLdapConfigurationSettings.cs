namespace NHSOnline.Backend.Support.Settings
{
    public class SpineLdapConfigurationSettings
    {
        public string LdapHost { get; set; }

        public int LdapPort { get; set; }

        public string LoginDN { get; set; }

        public string CertPath { get; set; }

        public string CertPassword { get; set; }

        public string NhsAppPartyId { get; set; }

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
            if (ldapEnabled)
            {
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
        }
    }
}
