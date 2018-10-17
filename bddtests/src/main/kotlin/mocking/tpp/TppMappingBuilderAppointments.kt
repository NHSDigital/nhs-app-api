package mocking.tpp

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.tpp.appointments.AppointmentSlotsBuilderTpp
import mocking.tpp.appointments.BookAppointmentsBuilderTpp
import mocking.tpp.appointments.CancelAppointmentsBuilderTpp
import mocking.tpp.appointments.MyAppointmentsBuilderTpp
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class TppMappingBuilderAppointments: IAppointmentMappingBuilder {

    override fun appointmentSlotsRequest(patient: Patient, fromDateTime: String?, toDateTime: String?)
            = AppointmentSlotsBuilderTpp(patient.tppUserSession!!, fromDateTime, toDateTime)

    override fun viewMyAppointmentsRequest(patient: Patient): IMyAppointmentsBuilder = MyAppointmentsBuilderTpp(patient)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade):
            IBookAppointmentsBuilder = BookAppointmentsBuilderTpp(patient, request)

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade):
            ICancelAppointmentsBuilder = CancelAppointmentsBuilderTpp(patient, request)

}