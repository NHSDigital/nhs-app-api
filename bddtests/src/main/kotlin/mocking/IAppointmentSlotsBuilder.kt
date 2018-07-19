package mocking


import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.Duration


interface IAppointmentSlotsBuilder {

    fun respondWithSuccess(slots: ArrayList<AppointmentSlotFacade>, sessionId: Int?, sessionDate: String?): Mapping

    fun withDelay(delayMilliseconds : Duration): IAppointmentSlotsBuilder

    fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping

    fun respondWithSuccess(body: String): Mapping

    fun respondWithExceptionWhenNotEnabled(): Mapping

    fun respondWithUnknownException(): Mapping

}