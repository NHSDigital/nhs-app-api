using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationDataMaps : IOrganDonationDataMaps
    {
        private const string Other = "other";

        public OrganDonationDataMaps()
        {
            {
                TitleDataMap = new Dictionary<string, string>
                {
                    { "MR", "MR" },
                    { "MRS", "MRS" },
                    { "MISS", "MISS" },
                    { "MS", "MS" },
                    { "MX", "MX" },
                    { "MASTER", "MASTER" },
                    { "DOCTOR", "DR" },
                    { "DR", "DR" },
                    { "CLLR", "CLLR" },
                    { "COUNCILLOR", "CLLR" },
                    { "CAPT", "CAPT" },
                    { "CAPTAIN", "CAPT" },
                    { "COLONEL", "COLONEL" },
                    { "EXORS", "EXORS" },
                    { "EXECUTORS OF", "EXORS" },
                    { "FR", "FR" },
                    { "FATHER", "FR" },
                    { "LADY", "LADY" },
                    { "LORD", "LORD" },
                    { "PROFESSOR", "PROF" },
                    { "REV", "REV" },
                    { "REVEREND", "REV" },
                    { "SIR", "SIR" },
                    { "DAME", "DAME" }
                };
                
                GenderDataMap = new Dictionary<string, string>
                {
                    { "MALE", "male" },
                    { "FEMALE", "female" },
                    { "TRANSGENDER", Other },
                    { "NOTKNOWN", Other },
                    { "INDETERMINATE", Other },
                    { "NOTSPECIFIED", Other },
                    { "UNSPECIFIED", Other }
                };
            }
        }

        public IDictionary<string, string> TitleDataMap { get; }
        
        public IDictionary<string, string> GenderDataMap { get; }
    }
}