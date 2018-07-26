package mocking.gpServiceBuilderInterfaces.appointments

import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

interface IAppointmentMappingBuilder {
    fun viewMyAppointmentsRequest(patient: Patient):IMyAppointmentsBuilder

    fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder

    fun appointmentSlotsRequest(patient: Patient, fromDateTime: String? = null, toDateTime: String? = null) : IAppointmentSlotsBuilder

    fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade): ICancelAppointmentsBuilder
}