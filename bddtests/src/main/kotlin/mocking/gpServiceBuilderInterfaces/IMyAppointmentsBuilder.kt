package mocking.gpServiceBuilderInterfaces

import mocking.models.Mapping

interface IMyAppointmentsBuilder {
    fun respondWithSuccess(body: String): Mapping
}