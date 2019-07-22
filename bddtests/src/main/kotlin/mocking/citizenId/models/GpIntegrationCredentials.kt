package mocking.citizenId.models

import com.google.gson.annotations.SerializedName

data class GpIntegrationCredentials
(
        @SerializedName("gp_ods_code")
        var OdsCode: String
)