using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public class LdapConnectionService : ILdapConnectionService
    {
        private ILogger<LdapConnectionService> _logger;
        private SpineLdapConfigurationSettings _configurationSettings;

        public LdapConnectionService(ILogger<LdapConnectionService> logger, SpineLdapConfigurationSettings configurationSettings)
        {
            _logger = logger;
            _configurationSettings = configurationSettings;
        }
        
        public ILdapConnection CreateLdapConnection()
        {
            var ldapConnection = new LdapConnection
            {
                SecureSocketLayer = true,
            };

            ldapConnection.UserDefinedClientCertSelectionDelegate += ClientCertificateSelectionHandler;
            ldapConnection.UserDefinedServerCertValidationDelegate += ServerCertificateValidationHandler;

            return ldapConnection;
        }

        public LdapAttributeSet Search(ILdapConnection conn, string dn, int scope, string filter)
        {
            var searchResult = conn.Search(_configurationSettings.LoginDN, LdapConnection.ScopeOne, filter, null, false, null);

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
                var rawData = ReadFile(_configurationSettings.CertPath);
                cert = new X509Certificate2(rawData, _configurationSettings.CertPassword);
                localcertificates.Add(cert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading or opening certificate from path {_configurationSettings.CertPath} using configured certificate password.");
            }

            return cert;
        }
    }
}
