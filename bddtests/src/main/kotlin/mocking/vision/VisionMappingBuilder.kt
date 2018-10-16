package mocking.vision

import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.vision.Demographics.VisionDemographicsBuilder
import mocking.vision.allergies.VisionAllergiesBuilder
import mocking.vision.Immunisations.VisionImmunisationsBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.defaults.MockDefaults
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.appointments.MyAppointmentsBuilderVision
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

open class VisionMappingBuilder(method: String = "POST") : MappingBuilder(method, "/vision/") {

    var appointments = VisionMappingBuilderAppointments()

    fun getConfigurationRequest(visionUserSession: VisionUserSession) =
            VisionGetConfigurationBuilder(visionUserSession, MockDefaults.visionGetConfiguration)

    fun getPrescriptionHistoryRequest(visionUserSession: VisionUserSession) =
            VisionGetHistoricPrescriptionsBuilder(visionUserSession, MockDefaults.visionGetHistory)

    fun getEligibleRepeatsRequest(visionUserSession: VisionUserSession) =
            VisionEligibleRepeatsBuilder(visionUserSession, MockDefaults.visionGetEligibleRepeats)

    fun orderNewPrescriptionRequest(
            visionUserSession: VisionUserSession,
            orderNewPrescriptionRequest: OrderNewPrescriptionRequest) =
            VisionOrderNewPrescriptionBuilder(
                    visionUserSession,
                    MockDefaults.visionOrderNewPrescription,
                    orderNewPrescriptionRequest
            )

    fun allergiesRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition) =
            VisionAllergiesBuilder(visionUserSession, serviceDefinition)

    fun demographicsRequest(visionUserSession: VisionUserSession) =
            VisionDemographicsBuilder(visionUserSession, MockDefaults.visionDemographicsConfiguration)

    fun immunisationsRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition) =
            VisionImmunisationsBuilder(visionUserSession, serviceDefinition)
}
