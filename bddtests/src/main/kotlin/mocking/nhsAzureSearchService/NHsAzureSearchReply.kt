package mocking.nhsAzureSearchService

import com.google.gson.annotations.SerializedName

data class NHSAzureSearchReply(
        var value: MutableList<NhsAzureSearchItem> = arrayListOf(),
        @SerializedName("@odata.count") var count: Int
)

data class NhsAzureSearchItem(
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