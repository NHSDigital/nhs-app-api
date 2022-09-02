using System;
using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Emis.Models.Records
{
    public class MedicalRecord
    {
        public string? PatientGuid { get; set; }
        public string? Title { get; set; }
        public string? Forenames { get; set; }
        public string? Surname { get; set; }
        public IList<Document> Documents {  get; } = new List<Document>();

        public MedicalRecord()
        {
            Documents = new List<Document>()
            {
                new()
                {
                    DocumentGuid = new Guid().ToString(),
                    Available = true,
                    Size = 500000,
                    Extension = "pdf"
                }
            };
        }
    }
}