package mocking.nhsAzureSearchService

import com.google.gson.annotations.SerializedName

data class NhsAzureSearchOrganisationReply(
        var value: MutableList<NhsAzureSearchOrganisationItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class Geocode(
        var Coordinates: MutableList<Double> = arrayListOf()
)

data class NhsAzureSearchOrganisationItem(
        var OrganisationName: String,
        var OrganisationType: String,
        var OrganisationSubType: String,
        var Address1: String,
        var Address2: String,
        var Address3: String,
        var City: String,
        var County: String,
        var Postcode: String,
        var ODSCode: String,
        var Geocode: Geocode,
        var Metrics: ArrayList<Metric> = arrayListOf(),
        var Contacts: ArrayList<Contact> = arrayListOf(),
        var URL: String? = null) {
    fun primaryPhone(): String? {
        if (!Contacts.isEmpty()) {
            val telephoneNumbers = Contacts.filter { it.ContactMethodType == "Telephone" }
            val primaryPhone = telephoneNumbers.firstOrNull()

            return primaryPhone?.ContactValue
        }
        return null
    }
}

data class Contact(
        var ContactType: String,
        var ContactAvailabilityType: String,
        var ContactMethodType: String,
        var ContactValue: String
)

data class Metric(
        var MetricName: String,
        var Value: String
)

data class NHSAzureSearchPostcodesAndPlacesReply(
        var value: MutableList<NhsAzureSearchPostcodesAndPlacesItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class NhsAzureSearchPostcodesAndPlacesItem(
        var Latitude: Int,
        var Longitude: Int
)
