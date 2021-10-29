package mocking.data.nhsAzureSearchData

import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NHSAzureSearchPostcodesAndPlacesReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesAndPlacesItem
import mocking.nhsAzureSearchService.Contact
import mocking.nhsAzureSearchService.Geocode
import mocking.nhsAzureSearchService.Metric

object NhsAzureSearchData {

    const val ORGANISATION_LIMIT = 20
    const val POSTCODES_AND_PLACES_LIMIT = 1
    private const val DEFAULT_LATITUDE = -10
    const val DEFAULT_LONGITUDE = 20
    // 1 less than F81090 in redis cache
    const val ORGANISATION_NAME = "Clay Cross Medical Centre"
    private const val PHARMACY_NAME = "My Pharmacy"

    private const val BASE_ODSCODE = 81089
    private const val POSTCODE_SEARCH_INDEX = 9

    fun generateOrganisationData(numberOfItems: Int): NhsAzureSearchOrganisationReply {
        val searchItems = mutableListOf<NhsAzureSearchOrganisationItem>()

        for(i in 1..numberOfItems.coerceAtMost(ORGANISATION_LIMIT)) {
            searchItems.add(createOrganisationResultFromIndex(i))
        }

        return NhsAzureSearchOrganisationReply(searchItems, numberOfItems)
    }

    fun generatePharmacyData(numberOfItems: Int): NhsAzureSearchOrganisationReply {
        val searchItems = mutableListOf<NhsAzureSearchOrganisationItem>()

        for(i in 1..numberOfItems.coerceAtMost(ORGANISATION_LIMIT)) {
            searchItems.add(createPharmacyResultFromIndex(i))
        }

        return NhsAzureSearchOrganisationReply(searchItems, numberOfItems)
    }

    fun getSuccessfulPostcodeMatch(): NHSAzureSearchPostcodesAndPlacesReply {
        val searchItems = mutableListOf<NhsAzureSearchPostcodesAndPlacesItem>()

        searchItems.add(NhsAzureSearchPostcodesAndPlacesItem(DEFAULT_LATITUDE, DEFAULT_LONGITUDE))

        return NHSAzureSearchPostcodesAndPlacesReply(searchItems, POSTCODES_AND_PLACES_LIMIT)
    }

    fun getOrganisationWithinRange(isPharmacySearch: Boolean = false): NhsAzureSearchOrganisationReply {
        val searchItems = mutableListOf<NhsAzureSearchOrganisationItem>()

        searchItems.add(createOrganisationResultFromIndex(POSTCODE_SEARCH_INDEX, isPharmacySearch))

        return NhsAzureSearchOrganisationReply(searchItems, POSTCODES_AND_PLACES_LIMIT)
    }

    private fun createOrganisationResultFromIndex(
            i: Int, isPharmacySearch: Boolean = false): NhsAzureSearchOrganisationItem {
        val numericODSCode = BASE_ODSCODE + i

        val organisationName = if (isPharmacySearch) "$PHARMACY_NAME $i" else "$ORGANISATION_NAME $i"

        val geocode = Geocode(
                Coordinates = mutableListOf(
                        DEFAULT_LATITUDE.toDouble() + i, DEFAULT_LONGITUDE.toDouble() + i
                ))

        return NhsAzureSearchOrganisationItem(
                organisationName,
                "P1",
                "Community",
                "$i Bridge Street",
                "Clay Cross",
                "Address Line 3",
                "Chesterfield",
                "County",
                "SW$i ${i % POSTCODE_SEARCH_INDEX + POSTCODES_AND_PLACES_LIMIT}NG",
                "F$numericODSCode",
                geocode)
    }

    private fun createPharmacyResultFromIndex(i: Int): NhsAzureSearchOrganisationItem {
        val numericODSCode = BASE_ODSCODE + i
        val organisationName = "$PHARMACY_NAME $i"
        val geocode = Geocode(
                Coordinates = mutableListOf(
                DEFAULT_LATITUDE.toDouble() + i, DEFAULT_LONGITUDE.toDouble() + i
        ))

        return NhsAzureSearchOrganisationItem(
                organisationName,
                "P1",
                "Community",
                "$i Bridge Street",
                "Clay Cross",
                "Address Line 3",
                "Chesterfield",
                "County",
                "SW$i ${i % POSTCODE_SEARCH_INDEX + POSTCODES_AND_PLACES_LIMIT}NG",
                "F$numericODSCode",
                geocode,
                arrayListOf(Metric("Electronic prescription service", "Yes")),
                arrayListOf(Contact(
                        "Primary",
                        "Office hours",
                        "Telephone",
                        "01132433551 (ext. $i)")),
                "www.$organisationName.com")
    }
}
