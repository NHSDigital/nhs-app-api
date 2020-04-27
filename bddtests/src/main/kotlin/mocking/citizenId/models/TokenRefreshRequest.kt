package mocking.citizenId.models

data class TokenRefreshRequest(
        val grantType: String = "refresh_token",
        val client_assertion_type: String = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
        val refresh_token: String
        )