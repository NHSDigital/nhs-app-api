using System;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MyRecordDate
    {
        public DateTimeOffset? Value { get; set; }  
        public string DatePart { get; set; } 
    }
}