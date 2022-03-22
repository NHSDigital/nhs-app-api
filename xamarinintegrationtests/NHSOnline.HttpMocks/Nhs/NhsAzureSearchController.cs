using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Nhs.Models;

namespace NHSOnline.HttpMocks.Nhs
{
    [Route("nhs-search-indexes/service-search/search")]
    public class NhsAzureSearchController: Controller
    {
        [HttpPost]
        public IActionResult NhsAzureServiceSearchPost()
        {
            return Json(new
            {
                Value = new List<NhsAzureSearchOrganisationItem>
                {
                    new()
                    {
                        OrganisationName = "Pharmacy",
                        OrganisationType = "P1",
                        OrganisationSubType = "Community",
                        Address1 = "1 Bridge Street",
                        Address2 = "Clay Cross",
                        Address3 = "Address Line 3",
                        City = "Chesterfield",
                        County = "County",
                        Postcode = "SW1 1AA",
                        ODSCode = "ods_code",
                        Metrics = new List<Metric>
                        {
                            new()
                            {
                                MetricName = "Electronic prescription service",
                                Value = "Yes",
                            }
                        }
                    }
                }
            });
        }
    }
}