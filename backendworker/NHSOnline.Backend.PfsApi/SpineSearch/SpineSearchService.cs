using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public class SpineSearchService
    {
        ILogger<SpineSearchService> _logger;
        SpineLdapConfigurationSettings _configurationSettings;
        ILdapConnectionService _ldapConnectionService;

        public SpineSearchService(ILogger<SpineSearchService> logger, SpineLdapConfigurationSettings spineLdapConfigurationSettings, ILdapConnectionService ldapConnectionService)
        {
            _logger = logger;
            _configurationSettings = spineLdapConfigurationSettings;
            _ldapConnectionService = ldapConnectionService;
        }

        public NhsAppSpinePdsTraceProperties RetrieveSpinePropertiesForPdsTrace()
        {
            var pdsTraceProperties = new NhsAppSpinePdsTraceProperties();
            var ldapConnection = _ldapConnectionService.CreateLdapConnection();

            try
            {
                ldapConnection.Connect(_configurationSettings.LdapHost, _configurationSettings.LdapPort);
                ldapConnection.Bind(_configurationSettings.LoginDN, string.Empty);

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
                    $"(&(nhsMhsPartyKey={_configurationSettings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pdsquery:QUPA_IN000008UK02))",
                    new Dictionary<string, Action<NhsAppSpinePdsTraceProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.FromAsid = value },
                    },
                    pdsTraceProperties);
            }
            catch (LdapException e)
            {
                _logger.LogError(e.LdapErrorMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error making ldap lookup");
            }
            finally
            {
                ldapConnection.Disconnect();
            }

            return pdsTraceProperties;
        }

        public NhsAppSpinePdsUpdateProperties RetrieveSpinePropertiesForPdsUpdate()
        {
            var pdsUpdateProperties = new NhsAppSpinePdsUpdateProperties();
            var ldapConnection = _ldapConnectionService.CreateLdapConnection();

            try
            {
                ldapConnection.Connect(_configurationSettings.LdapHost, _configurationSettings.LdapPort);
                ldapConnection.Bind(_configurationSettings.LoginDN, string.Empty);

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
                    $"(&(nhsMhsPartyKey={_configurationSettings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))",
                    new Dictionary<string, Action<NhsAppSpinePdsUpdateProperties, string>>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        { "uniqueIdentifier", (config, value) => config.FromAsid = value },
                    },
                    pdsUpdateProperties);
            }
            catch (LdapException e)
            {
                _logger.LogError(e.LdapErrorMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error making ldap lookup");
            }
            finally
            {
                ldapConnection.Disconnect();
            }

            return pdsUpdateProperties;
        }

        private void DoLdapSearch<T>(ILdapConnection conn, string searchFilter, Dictionary<string, Action<T, string>> spineNhsAppValueSetters, T spineNhsAppProperties)
        {
            var attributeSet = _ldapConnectionService.Search(conn, _configurationSettings.LoginDN, LdapConnection.ScopeOne, searchFilter);

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
    }
}
