package mocking.thirdPartyProviders.accuRx

import mocking.MappingBuilder

open class AccuRxRequestBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "$relativePath") {

    fun accuRxTriageAdviceRequest() = AccuRxTriageAdviceRequestBuilder()
}
