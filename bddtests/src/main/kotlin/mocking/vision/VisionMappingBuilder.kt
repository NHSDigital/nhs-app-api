package mocking.vision

import mocking.MappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.patientSelected.TppPatientSelectedBuilder
import mocking.tpp.session.TppSessionBuilder
import mocking.tpp.viewPatientOverview.TppViewPatientOverviewBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import worker.models.demographics.TppUserSession

open class VisionMappingBuilder(private val method: String) : MappingBuilder(method, "/vision/") {

    fun getConfigurationRequest(visionUserSession: VisionUserSession, serviceDefinition: ServiceDefinition)
            = VisionGetConfigurationBuilder(visionUserSession, serviceDefinition)

}