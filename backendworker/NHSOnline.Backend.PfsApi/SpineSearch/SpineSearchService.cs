using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public class SpineSearchService : ISpineSearchService
    {
        readonly ILogger<SpineSearchService> _logger;
        readonly SpineLdapConfigurationSettings _ldapConfigurationSettings;
        readonly ILdapConnectionService _ldapConnectionService;

        public SpineSearchService(ILogger<SpineSearchService> logger, SpineLdapConfigurationSettings spineLdapConfigurationSettings, ILdapConnectionService ldapConnectionService)
        {
            _logger = logger;
            _ldapConfigurationSettings = spineLdapConfigurationSettings;
            _ldapConnectionService = ldapConnectionService;
        }

        public NhsAppSpinePdsTraceProperties RetrieveSpinePropertiesForPdsTrace()
        {
            _logger.LogInformation("Retrieving spine properties for PDS trace");

            var pdsTraceProperties = new NhsAppSpinePdsTraceProperties();
            var ldapConnection = CreateLdapConnection();

            try
            {
                _ldapConnectionService.ConnectAndBind(ldapConnection);

                DoLdapSearch(
                    ldapConnection,
                    $"(&(nhsIDCode=YEA)(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pdsquery:QUPA_IN000008UK02))",
                    new Dictionary<string, Action<NhsAppSpinePdsTraceProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.ToAsid = value },
                    },
                    pdsTraceProperties);

                DoLdapSearch(
                    ldapConnection,
                    $"(&(nhsMhsPartyKey={_ldapConfigurationSettings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pdsquery:QUPA_IN000008UK02))",
                    new Dictionary<string, Action<NhsAppSpinePdsTraceProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.FromAsid = value },
                    },
                    pdsTraceProperties);
            }
            catch (LdapException e)
            {
                _logger.LogError(e, "LdapException occurred doing ldap lookup");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error doing ldap lookup");
            }
            finally
            {
                ldapConnection.Disconnect();
            }

            _logger.LogInformation($"Spine properties for PDS trace: {JsonConvert.SerializeObject(pdsTraceProperties)}");
            return pdsTraceProperties;
        }

        public NhsAppSpinePdsUpdateProperties RetrieveSpinePropertiesForPdsUpdate()
        {
            _logger.LogInformation("Retrieving spine properties for PDS update");

            var pdsUpdateProperties = new NhsAppSpinePdsUpdateProperties();
            var ldapConnection = CreateLdapConnection();

            try
            {
                _ldapConnectionService.ConnectAndBind(ldapConnection);

                DoLdapSearch(
                    ldapConnection,
                    $"(&(nhsIDCode=YEA)(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))",
                    new Dictionary<string, Action<NhsAppSpinePdsUpdateProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.ToAsid = value },
                        { "nhsMHSPartyKey", (config, value) => config.ToPartyId = value },
                    },
                    pdsUpdateProperties);

                DoLdapSearch(
                    ldapConnection,
                    $"(&(nhsIDCode=YEA)(objectClass=nhsMHS)(nhsMHSSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))",
                    new Dictionary<string, Action<NhsAppSpinePdsUpdateProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "nhsMhsCPAId", (config, value) => config.CpaId = value },
                    },
                    pdsUpdateProperties);

                DoLdapSearch(
                    ldapConnection,
                    $"(&(nhsMhsPartyKey={_ldapConfigurationSettings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))",
                    new Dictionary<string, Action<NhsAppSpinePdsUpdateProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.FromAsid = value },
                    },
                    pdsUpdateProperties);
            }
            catch (LdapException e)
            {
                _logger.LogError(e, "LdapException occurred doing ldap lookup");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error doing ldap lookup");
            }
            finally
            {
                ldapConnection.Disconnect();
            }

            _logger.LogInformation($"Spine properties for PDS update: {JsonConvert.SerializeObject(pdsUpdateProperties)}");
            return pdsUpdateProperties;
        }

        private void DoLdapSearch<T>(ILdapConnection conn, string searchFilter, Dictionary<string, Action<T, string>> spineNhsAppValueSetters, T spineNhsAppProperties)
        {
            var attributeSet = _ldapConnectionService.Search(conn, _ldapConfigurationSettings.LoginDN, LdapConnection.ScopeOne, searchFilter);

            _logger.LogDebug($"DoLdapSearch - attribute count: {attributeSet?.Count}");

            if (attributeSet != null)
            {
                IEnumerator ienum = attributeSet.GetEnumerator();

                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;

                    if (spineNhsAppValueSetters.ContainsKey(attribute.Name))
                    {
                        var valueSetter = spineNhsAppValueSetters[attribute.Name];
                        valueSetter(spineNhsAppProperties, attribute.StringValue);
                    }
                }
            }
        }

        private ILdapConnection CreateLdapConnection()
        {
            _logger.LogInformation("Creating LDAP connection");
            var ldapConnection = _ldapConnectionService.CreateLdapConnection();

            _logger.LogInformation("LDAP connection created");

            return ldapConnection;
        }
    }
}
