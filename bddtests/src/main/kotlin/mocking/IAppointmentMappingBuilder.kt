package mocking

import mocking.IBookAppointmentsBuilder
import models.Patient
import worker.models.appointments.BookAppointmentSlotRequest

interface IAppointmentMappingBuilder{

    fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotRequest): IBookAppointmentsBuilder
}