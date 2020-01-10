using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class KnownServices
    {
        public List<KnownService> Services { get; set; }
    }

    public class KnownService
    {
        public string TitleKey { get; set; }
        
        public string AccessibleTitleKey { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), false)]
        public Service Service { get; set; }
        
        public bool AllowNativeInteraction { get; set; }
        
        public Uri Url { get; set; }
        
        public bool IsExternal { get; set; }
        
        public bool UseCustomTabs { get; set; }
        
        public List<PathInfo> PathInfo { get; set; }
    }

    public class PathInfo
    {
        public string TitleKey { get; set; }
        
        public string AccessibleTitleKey { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public Service Service { get; set; }
        
        public bool AllowNativeInteraction { get; set; }
        
        public bool ValidateSession { get; set; }

        public string Path { get; set; }

        public int MenuTab { get; set; }
    }
    
    public enum Service
    {
        [EnumMember(Value = "")] 
        Default,
        Account,
        AdminHelp,
        Appointments,
        DataPreferences,
        More,
        HotJar,
        MyRecord,
        Nhs111,
        NhsOnline,
        OrganDonation,
        Others,
        Prescriptions,
        Symptoms
    }
}