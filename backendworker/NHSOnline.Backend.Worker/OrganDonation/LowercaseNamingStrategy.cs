using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Serialization;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    [SuppressMessage("Microsoft.Design", "CA1308", Justification = "requires lowercase for serialization")]
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name) => name.ToLowerInvariant();
    }
}