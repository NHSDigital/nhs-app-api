package mocking.microtest.appointments

import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import org.apache.http.HttpStatus

class DeleteAppointmentsBuilderMicrotest(request: CancelAppointmentSlotFacade) : MicrotestMappingBuilder(
    method = "DELETE",
    relativePath = "/appointments"
),
    ICancelAppointmentsBuilder {

    init {
        val requestBody = DeleteAppointmentsRequestModel (
            appointmentId = request.slotId.toString(),
            cancelReason = request.cancellationReason)
        requestBuilder
            .andJsonBody(requestBody)
    }

    override fun respondWithCorrupted(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithSuccess(): Mapping {
        return respondWith(HttpStatus.SC_NO_CONTENT){}
    }
}