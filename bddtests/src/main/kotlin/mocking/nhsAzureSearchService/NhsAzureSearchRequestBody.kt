package mocking.nhsAzureSearchService

data class NhsAzureSearchRequestBody(val top: Int = 20,
                                     val search: String = "",
                                     val searchFields: String =  "OrganisationName,Postcode,City",
                                     val select: String = "OrganisationID,OrganisationName," +
                                             "Address1,Address2,Address3,City,County,Postcode,NACSCode",
                                     val filter: String = "OrganisationTypeID eq 'GPB'",
                                     val orderby: String = "OrganisationName")