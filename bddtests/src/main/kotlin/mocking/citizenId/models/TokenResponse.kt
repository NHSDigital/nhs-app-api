package mocking.citizenId.models

import com.google.gson.annotations.SerializedName

class TokenResponse(@SerializedName("access_token") var accessToken: String? = null)
