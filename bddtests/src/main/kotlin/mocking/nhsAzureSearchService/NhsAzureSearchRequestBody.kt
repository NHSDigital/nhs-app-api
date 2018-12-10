package mocking.nhsAzureSearchService

import mocking.data.nhsAzureSearchData.NhsAzureSearchData

data class NhsAzureSearchRequestBody(val top: String = NhsAzureSearchData.LIMIT.toString(),
                                     val search: String = "",
                                     val searchFields: String =  "OrganisationName,Address2,Address3,Postcode,City",
                                     val select: String = "OrganisationID,OrganisationName," +
                                             "Address1,Address2,Address3,City,County,Postcode,NACSCode",
                                     val filter: String = "OrganisationTypeID eq 'GPB'",
                                     val orderby: String = "OrganisationName",
                                     val queryType: String = "simple",
                                     val count: Boolean = true)