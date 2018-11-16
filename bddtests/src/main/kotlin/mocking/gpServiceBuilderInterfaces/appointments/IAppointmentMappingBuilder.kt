package mocking.gpServiceBuilderInterfaces.appointments

import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import java.time.ZonedDateTime

interface IAppointmentMappingBuilder {
    fun viewMyAppointmentsRequest(patient: Patient): IMyAppointmentsBuilder

    fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder

    fun appointmentSlotsRequest(patient: Patient,
                                fromDateTime: ZonedDateTime? = null,
                                toDateTime: ZonedDateTime? = null): IAppointmentSlotsBuilder

    fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade): ICancelAppointmentsBuilder
}