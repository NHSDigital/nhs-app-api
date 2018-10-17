package mocking.stubs.appointments

import mocking.MockingClient
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentBookingSlotForMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.appointmentNotFoundMatcher
import mocking.stubs.appointments.AppointmentMatchers.Companion.successMatcherForAppointments
import mocking.stubs.appointments.AppointmentMatchers.Companion.timeoutMatcherForAppointments
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.Duration

class BookAppoinmentStubs(private val patient: Patient,
                          private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
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
            var facade = BookAppointmentSlotFacade(patient.userPatientLinkToken,
                                                   appointmentBookingSlotForMatcher,
                                                   scenario.forMatcher)
            mockingClient.forEmis { scenario.getResponse(appointments.bookAppointmentSlotRequest(patient, facade)) }
        }
    }
}