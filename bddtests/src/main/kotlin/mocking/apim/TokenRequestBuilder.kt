package mocking.apim

import mocking.models.Mapping
import org.apache.http.HttpStatus

class TokenRequestBuilder
    : ApimMappingBuilder("POST","/oauth2/token") {

    fun returnAccessTokenResponse(): Mapping {
        val response = """
        {
            "access_token": "accessToken",
            "expires_in": "123",
            "token_type": "Bearer",
            "issued_token_type": "urn:ietf:params:oauth:token-type:access_token"
        } 
        """

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(response)
                .build()
        }
    }
}
