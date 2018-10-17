package mocking.gpServiceBuilderInterfaces.appointments

import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade

interface IMyAppointmentsBuilder {
    fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping

    fun respondWithSuccess(body: String): Mapping

    fun respondWithExceptionWhenNotEnabled(): Mapping

    fun respondWithUnknownException() : Mapping

    fun respondWithCorrupted(facade: MyAppointmentsFacade) : Mapping
}