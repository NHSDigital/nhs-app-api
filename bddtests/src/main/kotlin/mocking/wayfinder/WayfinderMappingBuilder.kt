package mocking.wayfinder

import mocking.MappingBuilder
import mocking.WiremockUrlMatch

open class WayfinderMappingBuilder(method: String = "GET", path: String = "")
    : MappingBuilder(method, path, WiremockUrlMatch.Url) {

    fun timeout() = EvaluateBuilder().returnAfterThirtySecondsForTimeout()

    fun internalServerError() = EvaluateBuilder().returnInternalServerError()

    fun referralsAndUpcomingAppointments() = EvaluateBuilder().returnReferralsAndUpcomingAppointments()

    fun noReferralsOrUpcomingAppointments() = EvaluateBuilder().returnNoReferralsOrUpcomingAppointments()

    fun referralsNoAppointments() = EvaluateBuilder().returnReferralsAndNoUpcomingAppointments()

    fun referralsAppointmentsPartialError() = EvaluateBuilder().returnReferralsAndUpcomingAppointmentsWithPartialError()

    fun referralsAppointmentsUnderMinimumAgeError() =
        EvaluateBuilder().returnReferralsAndUpcomingAppointmentsUnderAgeError()

    fun referralsAndUpcomingAppointmentsErs() = EvaluateBuilder().returnReferralsAndUpcomingAppointmentsErs()

    fun referralsAndUpcomingAppointmentsPkb() = EvaluateBuilder().returnReferralsAndUpcomingAppointmentsPkb()
}
