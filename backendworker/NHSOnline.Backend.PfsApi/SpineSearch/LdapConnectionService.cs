using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;
using Task = System.Threading.Tasks.Task;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public class LdapConnectionService : ILdapConnectionService
    {
        private readonly ILogger<LdapConnectionService> _logger;
        private readonly SpineLdapConfigurationSettings _spineConfigurationSettings;
        private readonly ConfigurationSettings _configurationSettings;
        private readonly ICertificateService _certificateService;

        public LdapConnectionService(
            ILogger<LdapConnectionService> logger,
            SpineLdapConfigurationSettings spineConfigurationSettings,
            ConfigurationSettings configurationSettings,
            ICertificateService certificateService)
        {
            _logger = logger;
            _spineConfigurationSettings = spineConfigurationSettings;
            _configurationSettings = configurationSettings;
            _certificateService = certificateService;
        }

        public ILdapConnection CreateLdapConnection()
        {
            var ldapConnection = new LdapConnection
            {
                SecureSocketLayer = true,
                ConnectionTimeout = _configurationSettings.DefaultHttpTimeoutSeconds * 1000,
            };

            ldapConnection.UserDefinedClientCertSelectionDelegate += ClientCertificateSelectionHandler;
            ldapConnection.UserDefinedServerCertValidationDelegate +=
                    _certificateService.ServerCertificateValidationHandler;
           
            return ldapConnection;
        }

        public void ConnectAndBind(ILdapConnection ldapConnection)
        {
            var connectTask = Task.Run(() =>
            {
                try
                {
                    ldapConnection.Connect(_spineConfigurationSettings.LdapHost, _spineConfigurationSettings.LdapPort);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Ldap connection error");
                    throw;
                }
            });

            _logger.LogInformation(
                $"Trying to connect to: {_spineConfigurationSettings.LdapHost}:{_spineConfigurationSettings.LdapPort} with timeout {_configurationSettings.DefaultHttpTimeoutSeconds} seconds");

            if (!connectTask.Wait(_configurationSettings.DefaultHttpTimeoutSeconds * 1000))
            {
                _logger.LogError("Error trying to connect");
                throw new TimeoutException(
                    $"Error trying to connect to: {_spineConfigurationSettings.LdapHost}:{_spineConfigurationSettings.LdapPort}");
            }

            _logger.LogInformation("Trying to bind.");
            ldapConnection.Bind(_spineConfigurationSettings.LoginDN, string.Empty);

            _logger.LogInformation("Bind completed.");
        }

        public LdapAttributeSet Search(ILdapConnection conn, string dn, int scope, string filter)
        {
            _logger.LogInformation("Doing LDAP search");

            var searchResult = conn.Search(_spineConfigurationSettings.LoginDN, LdapConnection.ScopeOne, filter, null, false, null);

            while (searchResult.HasMore())
            {
                var entry = searchResult.Next();
                return entry.GetAttributeSet();
            }

            return null;
        }
        
        private X509Certificate ClientCertificateSelectionHandler(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            X509Certificate2 cert = null;
            try
            {
                cert = _certificateService.GetCertificate(_spineConfigurationSettings.CertPath,
                _spineConfigurationSettings.CertPassword);
                localCertificates.Add(cert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading or opening certificate from path {_spineConfigurationSettings.CertPath} using configured certificate password.");
            }

            return cert;
        }
    }
}
