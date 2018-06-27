package mocking.citizenId.models

import com.google.gson.annotations.SerializedName

class UserProfileResponse(@SerializedName("ods_code") var odsCode: String, @SerializedName("im1_connection_token") var connectionToken: String)
