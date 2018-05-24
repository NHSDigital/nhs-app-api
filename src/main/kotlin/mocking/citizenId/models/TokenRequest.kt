package mocking.citizenId.models

data class TokenRequest(
        val codeVerifier: String,
        val grantType: String = "authorization_code",
        val redirectUri: String = "http://localhost:3000/auth-return",
        val clientId: String = "nhs-online-poc",
        val codeChallengeMethod: String = "S256",
        val code: String?
)