package mocking.wayfinder

import mocking.MappingBuilder

open class WayfinderMappingBuilder(method: String="GET", relativePath: String= "")
    : MappingBuilder(method, relativePath) {

    fun timeout() = EvaluateBuilder().returnAfterThirtySecondsForTimeout()

    fun internalServerError() = EvaluateBuilder().returnInternalServerError()

    fun referrals() = EvaluateBuilder().returnReferralsAndAppointments()

    fun noReferrals() = EvaluateBuilder().returnNoReferralsOrAppointments()
}
