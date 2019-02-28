package mocking.spine.ePS

import mocking.spine.SpineConfiguration
import mocking.spine.SpineMappingBuilder

const val HEADER_API_ACCEPT = "Accept"
const val HEADER_API_FROM_ASID = "Spine-From-Asid"
const val HEADER_API_USER_ID = "Spine-UserId"
const val HEADER_API_PROFILE_ID = "Spine-RoleProfileId"
const val VAL_APPLICATION_JSON = "application/json"
const val HEADER_EPS_TRACE_ID = "Eps-TraceId"

open class EPSMappingBuilder(method: String, relativePath: String = "")
    : SpineMappingBuilder(method, "$relativePath") {

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
}