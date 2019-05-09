package mocking.data.nhsAzureSearchData

import mocking.nhsAzureSearchService.NHSAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NHSAzureSearchPostcodesAndPlacesReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesAndPlacesItem

@Suppress("MaxLineLength")
object NhsAzureSearchData {

    const val ORGANISATION_LIMIT = 20
    const val POSTCODES_AND_PLACES_LIMIT = 1
    const val DEFAULT_LATITUDE = -10
    const val DEFAULT_LONGITUDE = 20
    // 1 less than F81090 in redis cache
    const val ORGANISATION_NAME = "Clay Cross Medical Centre"
    const val PHARMACY_NAME = "My Pharmacy"
    private const val BASE_NACSCODE = 81089
    private const val BASE_ORGANISATION_ID = 4648
    private const val POSTCODE_SEARCH_INDEX = 9

    fun generateOrganisationData(numberOfItems: Int): NHSAzureSearchOrganisationReply {
        val searchItems = mutableListOf<NhsAzureSearchOrganisationItem>()

        for(i in 1..Math.min(numberOfItems, NhsAzureSearchData.ORGANISATION_LIMIT)) {
            searchItems.add(createOrganisationResultFromIndex(i))
        }

        return NHSAzureSearchOrganisationReply(searchItems, numberOfItems)
    }

    fun getSuccessfulPostcodeMatch(): NHSAzureSearchPostcodesAndPlacesReply {
        val searchItems = mutableListOf<NhsAzureSearchPostcodesAndPlacesItem>()

        searchItems.add(NhsAzureSearchPostcodesAndPlacesItem(DEFAULT_LATITUDE, DEFAULT_LONGITUDE))

        return NHSAzureSearchPostcodesAndPlacesReply(searchItems, POSTCODES_AND_PLACES_LIMIT)
    }

    fun getOrganisationWithinRange(isPharmacySearch: Boolean = false): NHSAzureSearchOrganisationReply {
        val searchItems = mutableListOf<NhsAzureSearchOrganisationItem>()

        searchItems.add(createOrganisationResultFromIndex(POSTCODE_SEARCH_INDEX, isPharmacySearch))

        return NHSAzureSearchOrganisationReply(searchItems, POSTCODES_AND_PLACES_LIMIT)
    }

    private fun createOrganisationResultFromIndex(i: Int, isPharmacySearch: Boolean = false): NhsAzureSearchOrganisationItem {
        val numericNACSCode = BASE_NACSCODE + i
        val organisationId = BASE_ORGANISATION_ID + i

        val organisationName = if (isPharmacySearch) "$PHARMACY_NAME $i" else "$ORGANISATION_NAME $i"

        return NhsAzureSearchOrganisationItem("$organisationId", "$organisationName", "$i Bridge Street", "Clay " +
                "Cross", "", "Chesterfield", "County", "SW$i ${i % POSTCODE_SEARCH_INDEX +
                POSTCODES_AND_PLACES_LIMIT}NG", "F$numericNACSCode")
    }

}
