using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NHSOnline.HttpMocks.Spine
{
    public sealed class SpineOrganisation
    {
        public static SpineOrganisation CreateCCG(
            string name,
            string code,
            string lastChangeDate,
            string postcode)
        {
            return new SpineOrganisation
            {
                Name = name,
                OrgId = code,
                PostCode = postcode,
                LastChangeDate = lastChangeDate
            };
        }

        public static SpineOrganisation CreateGP(
            string name,
            string code,
            string lastChangeDate,
            string postcode,
            string ccgCode)
        {
            return new SpineOrganisation
            {
                Name = name,
                OrgId = code,
                OrgRecordClass = "RC1",
                PostCode = postcode,
                LastChangeDate = lastChangeDate,
                Rels = new JObject(new JProperty("Rel",
                    new List<JObject>
                    {
                        new JObject(
                            new JProperty("Status", "Active"),
                            new JProperty("Target",
                                new JObject(
                                    new JProperty("OrgId", new JObject(new JProperty("extension", ccgCode))),
                                    new JProperty("PrimaryRoleId", new JObject(new JProperty("Id", "RO98"))))))
                    }))
            };
        }

        public string OrgId { get; private set; }
        private string OrgRecordClass { get; set; }
        private string Name { get; set; }
        private string PostCode { get; set; }
        private string LastChangeDate { get; set; }
        private JObject Rels { get; set; } = new JObject();

        internal JObject SearchResult()
        {
            return new JObject(
                new JProperty(nameof(OrgId), OrgId),
                new JProperty(nameof(Name), Name),
                new JProperty("Status", "Active"),
                new JProperty(nameof(LastChangeDate), LastChangeDate),
                new JProperty(nameof(PostCode), PostCode),
                new JProperty(nameof(OrgRecordClass), OrgRecordClass));
        }

        public JObject Details()
        {
            return new JObject(
                new JProperty("Organisation", Organisation()));

            JObject Organisation()
            {
                {
                    return new JObject(
                        new JProperty(nameof(Name), Name),
                        new JProperty("Status", "Active"),
                        new JProperty(nameof(LastChangeDate), LastChangeDate),
                        new JProperty("Rels", Rels));
                }
            }
        }
    }
}