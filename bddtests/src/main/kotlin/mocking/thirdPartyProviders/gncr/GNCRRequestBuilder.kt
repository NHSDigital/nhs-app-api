package mocking.thirdPartyProviders.gncr
import mocking.MappingBuilder

open class GNCRRequestBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "$relativePath") {

    fun gncrAppointmentsRequest() = GNCRAppointmentsRequestBuilder()

    fun gncrCorrespondenceRequest() = GNCRCorrespondenceRequestBuilder()
}
