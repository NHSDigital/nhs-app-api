package mocking.citizenId.models

import config.Config

data class TokenRequest(
        val codeVerifier: String = ".*",
        val grantType: String = "authorization_code",
        val clientId: String = "nhs-online",
        val redirectUri: String = Config.instance.cidRedirectUri,
        val code: String?,
        val code_challenge_method: String = "S256"
        )
