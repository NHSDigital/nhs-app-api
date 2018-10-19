package mocking.vision

import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.vision.appointments.MyAppointmentsBuilderVision
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

open class VisionMappingBuilderAppointments(method: String = "POST") : MappingBuilder(method, "/vision/"),
        IAppointmentMappingBuilder {

    override fun viewMyAppointmentsRequest(patient: Patient):
            IMyAppointmentsBuilder = MyAppointmentsBuilderVision(patient)

    override fun bookAppointmentSlotRequest(patient: Patient, request: BookAppointmentSlotFacade):
            IBookAppointmentsBuilder = throw NotImplementedError("To be implemented as part of NHSO-796")

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: String?,
                                         toDateTime: String?):
            IAppointmentSlotsBuilder = throw NotImplementedError("To be implemented as part of NHSO-795")

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade):
            ICancelAppointmentsBuilder = throw NotImplementedError("To be implemented as part of NHSO-798")
}
