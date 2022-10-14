package mocking.thirdPartyProviders.patchs
import mocking.MappingBuilder

open class PatchsRequestBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "$relativePath") {

    fun patchsMedicalRequest() = PatchsMedicalRequestBuilder()
}
