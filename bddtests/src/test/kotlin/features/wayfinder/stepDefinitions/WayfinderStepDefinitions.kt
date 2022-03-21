package features.wayfinder.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import utils.SerenityHelpers

class WayfinderStepDefinitions {
    @Given("^I am a user who can view Wayfinder from Appointments$")
    fun iAmAUserWhoCanViewWellnessAndPreventionFromHealthRecordHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        setupJourney(patient)
    }

    private fun setupJourney(patient: Patient) {
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
