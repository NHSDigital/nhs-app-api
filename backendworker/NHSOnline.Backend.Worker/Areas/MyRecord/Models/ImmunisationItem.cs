using System;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ImmunisationItem
    {
        public string Term { get; set; }
        public MyRecordDate EffectiveDate { get; set; }
    }
}