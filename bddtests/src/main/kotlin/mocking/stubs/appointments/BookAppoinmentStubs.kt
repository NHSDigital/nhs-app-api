package mocking.stubs.appointments

import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.TppStubsPatientFactory
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentBookingSlotForMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentConflictMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentLimitReachedMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentNotFoundMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.successMatcherForAppointments
import mocking.stubs.appointments.AppointmentMatchers.Companion.timeoutMatcherForAppointments
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.Duration

class BookAppoinmentStubs(private val mockingClient: MockingClient, private val patient: Patient ?= null) {
    fun generateStubs(supplier: String){
        when (supplier){
            "EMIS" -> generateEMISStubs()
            "TPP" -> generateTPPStubs()
        }
    }

    private fun generateEMISStubs() {
        val mapBookAppointmentStubs =
                InputResponse<String, IBookAppointmentsBuilder>()
                        .addResponse(successMatcherForAppointments)
                        {
                            builder -> builder.respondWithSuccess()
                        }

                        .addResponse(appointmentNotFoundMatcher)
                        {
                            builder ->
                            builder.respondWithExceptionWhenNotAvailable()
                        }

                        .addResponse(timeoutMatcherForAppointments)
                        {
                            builder ->
                            builder.withDelay(Duration.ofSeconds(TIMEOUT_DELAY)).respondWithSuccess()
                        }

        mapBookAppointmentStubs.listResponse().forEach { scenario ->
            val facade = BookAppointmentSlotFacade(patient!!.userPatientLinkToken,
                                                   appointmentBookingSlotForMatcher,
                                                   scenario.forMatcher)
            mockingClient.forEmis { scenario.getResponse(appointments.bookAppointmentSlotRequest(patient, facade)) }
        }
    }

    private fun generateTPPStubs() {
        val mapBookAppointmentStubs =
                InputResponse<String, IBookAppointmentsBuilder>()
                        .addResponse(successMatcherForAppointments)
                        {
                            builder -> builder.respondWithSuccess()
                        }

                        .addResponse(timeoutMatcherForAppointments)
                        {
                            builder -> builder.withDelay(Duration.ofSeconds(TIMEOUT_DELAY)).respondWithSuccess()
                        }

                        .addResponse(appointmentNotFoundMatcher)
                        {
                            builder -> builder.respondWithExceptionWhenNotAvailable()
                        }

                        .addResponse(appointmentConflictMatcher)
                        {
                            builder -> builder.respondWithConflictException()
                        }

                        .addResponse(appointmentLimitReachedMatcher)
                        {
                            builder -> builder.respondWithBookingLimitException()
                        }

        mapBookAppointmentStubs.listResponse().forEach { scenario ->
            val facade = BookAppointmentSlotFacade(TppStubsPatientFactory.goodPatientTPP.userPatientLinkToken,
                    appointmentBookingSlotForMatcher,
                    scenario.forMatcher)

            mockingClient.forTpp { scenario.getResponse(appointments.bookAppointmentSlotRequest(
                    TppStubsPatientFactory.goodPatientTPP, facade))}
        }
    }
}