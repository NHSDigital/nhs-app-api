package mocking.citizenId.models

import config.Config

data class TokenRequest(
        val codeVerifier: String,
        val grantType: String = "authorization_code",
        val clientId: String = "nhs-online-poc",
        val redirectUri: String = Config.instance.cidRedirectUri,
        val codeChallengeMethod: String = "S256",
        val code: String?
)
