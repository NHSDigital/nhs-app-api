package mocking.gpServiceBuilderInterfaces.appointments

import mocking.models.Mapping

interface ICancelAppointmentsBuilder {

    fun respondWithSuccess(): Mapping

    fun respondWithCorrupted(): Mapping

    fun responseWithExceptionWhenServiceUnavailable(): Mapping

    fun responseErrorForbiddenService(): Mapping

    fun respondWithExceptionWhenNotAvailable(): Mapping

    fun respondWithWithinAnHour(): Mapping

    fun respondWithUnknownException(): Mapping
}