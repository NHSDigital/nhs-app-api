package mocking.nhsAzureSearchService

import com.google.gson.annotations.SerializedName

data class NHSAzureSearchOrganisationReply(
        var value: MutableList<NhsAzureSearchOrganisationItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class NhsAzureSearchOrganisationItem(
        var OrganisationID: String,
        var OrganisationName: String,
        var Address1: String,
        var Address2: String,
        var Address3: String,
        var City: String,
        var County: String,
        var Postcode: String,
        var NACSCode: String
)

data class NHSAzureSearchPostcodesAndPlacesReply(
        var value: MutableList<NhsAzureSearchPostcodesAndPlacesItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int = 0
)

data class NhsAzureSearchPostcodesAndPlacesItem(
        var Latitude: Int,
        var Longitude: Int
)