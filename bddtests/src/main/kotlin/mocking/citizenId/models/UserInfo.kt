package mocking.citizenId.models

import com.google.gson.annotations.SerializedName

data class UserInfo (
        @SerializedName("given_name")
        var GivenName: String,

        @SerializedName("family_name")
        var FamilyName: String,

        @SerializedName("gp_integration_credentials")
        var GpIntegrationCredentials: GpIntegrationCredentials,

        @SerializedName("nhs_number")
        var NhsNumber: String,

        @SerializedName("im1_token")
        var Im1ConnectionToken: String,

        @SerializedName("sub")
        var Subject: String,

        @SerializedName("birthdate")
        var Birthdate: String,

        @SerializedName("identity_proofing_level")
        var IdentityProofingLevel: String?,

        @SerializedName("email")
        val Email: String
)