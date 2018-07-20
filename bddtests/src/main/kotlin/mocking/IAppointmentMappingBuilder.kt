package mocking

import mocking.gpServiceBuilderInterfaces.IMyAppointmentsBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

interface IAppointmentMappingBuilder {
    fun viewAppointment(patient: Patient):IMyAppointmentsBuilder

    fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder

    fun appointmentSlotsRequest(patient: Patient, fromDateTime: String? = null, toDateTime: String? = null) : IAppointmentSlotsBuilder
}