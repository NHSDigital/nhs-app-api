using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Mappers
{
    public class OlcDataMaps : IOlcDataMaps
    {

        public OlcDataMaps()
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
            }
        }

        public IDictionary<string, string> TitleDataMap { get; }
    }
}