using System;

namespace NHSOnline.HttpMocks.Emis.Models.Records
{
    public class Document
    {
        public string? DocumentGuid { get; set; } = new Guid().ToString();
        public int? Size { get; set; }
        public string? Extension { get; set; } = null!;
        public bool Available { get; set; } = true;
    }
}