package mocking.apim

import mocking.MappingBuilder

open class ApimMappingBuilder(method: String="POST", relativePath: String= "")
    : MappingBuilder(method, relativePath) {

    fun successfulTokenRequest() = TokenRequestBuilder().returnAccessTokenResponse()
}
