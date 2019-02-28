package mocking.spine

import mocking.MappingBuilder
import mocking.spine.ePS.EPSMappingBuilder
import mocking.spine.ePS.EPSMappingBuilder111ItemDetail
import mocking.spine.ePS.EPSMappingBuilder111ItemSummary

open class SpineMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, "/spine$relativePath") {

    companion object {
        val configuration = SpineConfiguration("", "", "", "")
    }

    init {
        requestBuilder
        //.andHeader(HEADER_API_ACCEPT, VAL_APPLICATION_JSON)
        //.andHeader(HEADER_API_FROM_ASID, configuration.fromAsid)
        //.andHeader(HEADER_API_USER_ID, configuration.userId)
        //.andHeader(HEADER_API_PROFILE_ID, configuration.roleProfileId)
        //.andHeader(HEADER_EPS_TRACE_ID, configuration.epsTraceId)

    }

    var itemSummary = EPSMappingBuilder111ItemSummary(EPSMappingBuilder.configuration)
    var itemDescription = EPSMappingBuilder111ItemDetail(EPSMappingBuilder.configuration)
}