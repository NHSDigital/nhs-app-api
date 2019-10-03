using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
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

        public LdapConnectionService(
            ILogger<LdapConnectionService> logger,
            SpineLdapConfigurationSettings spineConfigurationSettings,
            ConfigurationSettings configurationSettings)
        {
            _logger = logger;
            _spineConfigurationSettings = spineConfigurationSettings;
            _configurationSettings = configurationSettings;
        }

        public ILdapConnection CreateLdapConnection()
        {
            var ldapConnection = new LdapConnection
            {
                SecureSocketLayer = true,
                ConnectionTimeout = _configurationSettings.DefaultHttpTimeoutSeconds * 1000,
            };

            ldapConnection.UserDefinedClientCertSelectionDelegate += ClientCertificateSelectionHandler;
            ldapConnection.UserDefinedServerCertValidationDelegate += ServerCertificateValidationHandler;

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

        private bool ServerCertificateValidationHandler(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // Overriding the default server certificate validation from:
            //   https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard/blob/master/src/Novell.Directory.Ldap.NETStandard/Connection.cs#L475
            // The code is the same but we have added logs to help get to the bottom of certificate problems, if we encounter them.

            var success = false;

            _logger.LogInformation($"ServerCertificateValidationHandler - SslPolicyErrors: {sslPolicyErrors}");

            if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    _logger.LogError($"SSL policy errors={sslPolicyErrors.ToString()}, continuing");
                    success = true;
                }
                else
                {
                    foreach (var item in chain.ChainStatus)
                    {
                        _logger.LogError($"Certificate validation was not successful. " +
                            $"SSL policy errors={sslPolicyErrors.ToString()}, Status={item.Status}, StatusInformation={item.StatusInformation}");
                    }
                }
            }
            else
            {
                success = true;
            }

            return success;
        }

        private static byte[] ReadFile(string fileName)
        {
            FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int size = (int)f.Length;
            byte[] data = new byte[size];
            size = f.Read(data, 0, size);
            f.Close();
            return data;
        }

        private X509Certificate ClientCertificateSelectionHandler(object sender, string targethost, X509CertificateCollection localcertificates, X509Certificate remotecertificate, string[] acceptableissuers)
        {
            X509Certificate2 cert = null;
            try
            {
                var rawData = ReadFile(_spineConfigurationSettings.CertPath);
                cert = new X509Certificate2(rawData, _spineConfigurationSettings.CertPassword);
                localcertificates.Add(cert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading or opening certificate from path {_spineConfigurationSettings.CertPath} using configured certificate password.");
            }

            return cert;
        }
    }
}
