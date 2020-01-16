package mocking.nhsAzureSearchService

import com.google.gson.Gson
import com.google.gson.annotations.SerializedName

data class NhsAzureSearchOrganisationReply(
        var value: MutableList<NhsAzureSearchOrganisationItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class Geocode(
        var Coordinates: MutableList<Double> = arrayListOf()
)

data class NhsAzureSearchOrganisationItem(
        var OrganisationID: String,
        var OrganisationName: String,
        var OrganisationType: String,
        var OrganisationSubType: String,
        var Address1: String,
        var Address2: String,
        var Address3: String,
        var City: String,
        var County: String,
        var Postcode: String,
        var NACSCode: String,
        var Geocode: Geocode,
        var Metrics: String? = null,
        var Contacts: String? = null,
        var URL: String? = null
) {
        fun addressFormatted(): String {
            val addressElements = listOfNotNull(Address1, Address2, Address3, City, County, Postcode)
            return addressElements.stream().filter{item->item.isNotEmpty()}.toArray().joinToString(", ")
        }

    fun primaryPhone(): String? {
        if (Contacts != null) {
            val contacts = Gson().fromJson(Contacts, Array<Contact>::class.java)

            val telephoneNumbers = contacts.filter { it.OrganisationContactMethodType == "Telephone" }
            val primaryPhone = telephoneNumbers.firstOrNull()

            return primaryPhone?.OrganisationContactValue
        }
        return null
    }
}

data class Contact(
        var OrganisationContactType: String,
        var OrganisationContactAvailabilityType: String,
        var OrganisationContactMethodType: String,
        var OrganisationContactValue: String
        )

data class NHSAzureSearchPostcodesAndPlacesReply(
        var value: MutableList<NhsAzureSearchPostcodesAndPlacesItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class NhsAzureSearchPostcodesAndPlacesItem(
        var Latitude: Int,
        var Longitude: Int
)
