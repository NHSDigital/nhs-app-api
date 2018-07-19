package mocking

import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient

interface IAppointmentMappingBuilder{

    fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade): IBookAppointmentsBuilder
}