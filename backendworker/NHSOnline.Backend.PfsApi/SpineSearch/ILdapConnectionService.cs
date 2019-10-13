using Novell.Directory.Ldap;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public interface ILdapConnectionService
    {
        ILdapConnection CreateLdapConnection();

        void ConnectAndBind(ILdapConnection ldapConnection);

        LdapAttributeSet Search(ILdapConnection conn, string dn, int scope, string filter);
    }
}
