package features.wayfinder.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.wayfinder.factories.WayfinderFactory
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.wayfinder.WayfinderReferralsAndAppointmentsPage
import utils.SerenityHelpers

class WayfinderStepDefinitions {
    @Steps
    lateinit var wayfinderReferralsAndAppointmentsPage: WayfinderReferralsAndAppointmentsPage
    private val wayfinderFactory = WayfinderFactory()

    @Given("^I am a user who can view Wayfinder from Appointments$")
    fun iAmAUserWhoCanViewWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupNoReferralsOrAppointmentsResponse();
    }

    @Given("^I am a user who can view Wayfinder from Appointments and has referrals$")
    fun iAmAUserWhoCanViewReferralsInWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsResponse()
    }

    @Then("^the Referrals, hospital and other appointments screen with data is displayed$")
    fun assertReferralsAndAppointmentsWithDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderWithDataTitleIsDisplayed()
    }

    @Then("^the Referrals, hospital and other appointments screen without data is displayed$")
    fun assertReferralsAndAppointmentsWithoutDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderWithoutDataTitleIsDisplayed()
    }

    @Then("^I see an in review referral$")
    fun assertInReviewReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertInReviewReferralDisplayed()
    }

    @Then("^I see a bookable cancelled referral$")
    fun assertBookableCancelledReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookableCancelledReferralDisplayed()
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
