package mocking.nhsAzureSearchService

import mocking.data.nhsAzureSearchData.NhsAzureSearchData

data class NhsAzureSearchRequestBody(val top: Int = NhsAzureSearchData.LIMIT,
                                     val search: String = "",
                                     val searchFields: String =  "OrganisationName,Postcode,City",
                                     val select: String = "OrganisationID,OrganisationName," +
                                             "Address1,Address2,Address3,City,County,Postcode,NACSCode",
                                     val filter: String = "OrganisationTypeID eq 'GPB'",
                                     val orderby: String = "OrganisationName",
                                     val queryType: String = "simple",
                                     val count: Boolean = true)