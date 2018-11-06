package mocking.gpServiceBuilderInterfaces.appointments


import mocking.gpServiceBuilderInterfaces.IBuilderCommonResponses
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.Duration


interface IAppointmentSlotsBuilder:IBuilderCommonResponses {

    fun withDelay(delayMilliseconds : Duration): IAppointmentSlotsBuilder

    fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping

    fun respondWithExceptionWhenNotEnabled(): Mapping
}