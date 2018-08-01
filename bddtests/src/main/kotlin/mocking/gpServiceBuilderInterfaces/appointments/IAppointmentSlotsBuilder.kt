package mocking.gpServiceBuilderInterfaces.appointments


import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.Duration


interface IAppointmentSlotsBuilder {

    fun withDelay(delayMilliseconds : Duration): IAppointmentSlotsBuilder

    fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping

    fun respondWithExceptionWhenNotEnabled(): Mapping

    fun respondWithUnknownException(): Mapping

}