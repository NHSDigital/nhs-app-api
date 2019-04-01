package mocking.microtest

import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.microtest.appointments.AppointmentSlotsBuilderMicrotest
import mocking.microtest.appointments.BookAppointmentsBuilderMicrotest
import mocking.microtest.appointments.GetAppointmentBuilderMicrotest
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import java.time.ZonedDateTime

class MicrotestMappingBuilderAppointments : IAppointmentMappingBuilder {

    override fun viewMyAppointmentsRequest(
            patient: Patient,
            appointmentType: IMyAppointmentsBuilder.AppointmentType
    ): IMyAppointmentsBuilder = GetAppointmentBuilderMicrotest(patient)


    override fun bookAppointmentSlotRequest(
            patient: Patient,
            request: BookAppointmentSlotFacade
    ): IBookAppointmentsBuilder {
        return BookAppointmentsBuilderMicrotest(request)
    }

    override fun cancelAppointmentRequest(
            patient: Patient,
            request: CancelAppointmentSlotFacade
    ): ICancelAppointmentsBuilder {
        TODO("not implemented")
    }

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: ZonedDateTime?,
                                         toDateTime: ZonedDateTime?) = AppointmentSlotsBuilderMicrotest(
            fromDateTime,
            toDateTime)
}