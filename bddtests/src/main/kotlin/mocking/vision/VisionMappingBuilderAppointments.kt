package mocking.vision

import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.vision.appointments.AppointmentSlotsBuilderVision
import mocking.vision.appointments.BookAppointmentsBuilderVision
import mocking.vision.appointments.CancelAppointmentBuilderVision
import mocking.vision.appointments.MyAppointmentsBuilderVision
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import java.time.ZonedDateTime

open class VisionMappingBuilderAppointments(method: String = "POST") : MappingBuilder(method, "/vision/pfs/"),
        IAppointmentMappingBuilder {

    override fun viewMyAppointmentsRequest(patient: Patient, appointmentType: IMyAppointmentsBuilder.AppointmentType):
            IMyAppointmentsBuilder = MyAppointmentsBuilderVision(patient)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade):
            IBookAppointmentsBuilder = BookAppointmentsBuilderVision(patient, request)

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: ZonedDateTime?,
                                         toDateTime: ZonedDateTime?):
            IAppointmentSlotsBuilder = AppointmentSlotsBuilderVision(patient, fromDateTime, toDateTime)

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade):
            ICancelAppointmentsBuilder = CancelAppointmentBuilderVision(patient, request)
}
