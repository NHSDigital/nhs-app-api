package mocking.nhsAzureSearchService

import mocking.data.nhsAzureSearchData.NhsAzureSearchData

const val FILTER_LOCAL_TYPE_POSTCODE = "LocalType eq 'Postcode'"

data class NhsAzureSearchOrganisationWithPostcodeRequestBody(
        val top: Int = NhsAzureSearchData.ORGANISATION_LIMIT,
        val select: String = "OrganisationName,Address1,Address2,Address3,City,County,Postcode," +
                "ODSCode",
        val filter: String = "OrganisationTypeId eq 'GPB'",
        val search: String? = "",
        val searchFields: String? = "OrganisationName,Address2,Address3,City",
        val count: Boolean = true,
        val orderby: String? = null,
        val queryType: String? = "simple",
        val searchMode: String? = null
)

data class NhsAzureSearchOrganisationRequestBody(
        val top: Int = NhsAzureSearchData.ORGANISATION_LIMIT,
        val search: String? = "",
        val searchFields: String? = "OrganisationName,Address2,Address3,City",
        val select: String? = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode," +
                "ODSCode",
        val filter: String = "OrganisationTypeId eq 'GPB'",
        val queryType: String? = "simple",
        val count: Boolean = true,
        val searchMode: String? = null
)

data class NhsAzureSearchPostcodesAndPlacesRequestBody(
        val top: Int = NhsAzureSearchData.POSTCODES_AND_PLACES_LIMIT,
        val search: String = "",
        val filter: String = FILTER_LOCAL_TYPE_POSTCODE,
        val count: Boolean = true
)

fun nhsAzureSearchOrganisationByOdsCodeRequestBody(odsCode: String): NhsAzureSearchOrganisationRequestBody {
    return NhsAzureSearchOrganisationRequestBody(
            top = 1,
            filter = "OrganisationTypeId eq 'GPB' and ODSCode eq '$odsCode'",
            select = "OrganisationName,ODSCode,Metrics",
            search = null,
            count = false,
            searchFields = null,
            searchMode = null,
            queryType = null
    )
}
