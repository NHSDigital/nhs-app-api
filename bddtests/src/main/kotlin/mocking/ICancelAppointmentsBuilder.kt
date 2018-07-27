package mocking

import mocking.emis.appointments.DeleteAppointmentResponseModel
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import org.apache.http.HttpStatus

interface ICancelAppointmentsBuilder {

    fun respondWithSuccess(): Mapping
}