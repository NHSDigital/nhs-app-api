package mocking.gpServiceBuilderInterfaces.appointments

import mocking.models.Mapping

interface IMyAppointmentsBuilder {
    fun respondWithSuccess(body: String): Mapping
}