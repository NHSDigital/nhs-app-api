package mocking.nhsAzureSearchService

import config.Config
import mocking.data.nhsAzureSearchData.NhsAzureSearchData

fun getGeoDistanceFilterForLatLon(lat: Int, lon: Int): String {
    return "OrganisationTypeID eq 'GPB' and geo.distance(Geocode, geography'POINT($lon $lat)') le ${Config
            .instance.postcodeLookupSearchRadiusKm}"
}

fun getGeoDistanceOrderbyForLatLon(lat: Int, lon: Int): String {
    return "geo.distance(Geocode, geography'POINT($lon $lat)')"
}

const val FILTER_LOCAL_TYPE_POSTCODE = "LocalType eq 'Postcode'"
const val FILTER_TYPE_POSTCODE_OUT_CODE = "Type eq 'PostcodeOutCode'"
const val SELECT_ORGANISATIONS_GEOCODE_SEARCH = "OrganisationID,OrganisationName,Address1,Address2,Address3,City," +
        "County,Postcode,NACSCode,Geocode"

data class NhsAzureSearchOrganisationWithPostcodeRequestBody(
        val top: Int = NhsAzureSearchData.ORGANISATION_LIMIT,
        val select: String = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode," +
                "NACSCode",
        val filter: String = "OrganisationTypeID eq 'GPB'",
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
                "NACSCode",
        val filter: String = "OrganisationTypeID eq 'GPB'",
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
            filter = "OrganisationTypeID eq 'GPB' and NACSCode eq '$odsCode'",
            select = "OrganisationID,OrganisationName,NACSCode,Metrics",
            search = null,
            count = false,
            searchFields = null,
            searchMode = null,
            queryType = null
    )
}
