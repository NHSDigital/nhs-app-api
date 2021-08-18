using System;

namespace Nhs.App.Api.Integration.Tests.Models
{
    public class EventReportCreateRequest
    {
        public string SupplierId { get; set; }
        public DateTime ReportDate { get; set; }
    }
}
