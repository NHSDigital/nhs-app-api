package mocking.thirdPartyProviders.substrakt
import mocking.MappingBuilder

open class SubstraktRequestBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "/jump$relativePath") {

    fun substraktMessageRequest() = SubstraktMessagesRequestBuilder()

    fun substraktAccountAdminRequest() = SubstraktAccountAdminRequestBuilder()

    fun substraktParticipationRequest() = SubstraktParticipationRequestBuilder()

}
