package features.wayfinder.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.BrowserSteps
import features.wayfinder.factories.WayfinderFactory
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import net.thucydides.core.annotations.Steps
import pages.wayfinder.WayfinderAggregatorErrorPage
import pages.wayfinder.WayfinderReferralsAndAppointmentsPage
import pages.wayfinder.help.ReferralsOrAppointmentsHelpPage
import pages.wayfinder.help.ConfirmedAppointmentsHelpPage
import pages.wayfinder.help.ReferralsInReviewHelpPage
import utils.SerenityHelpers

class WayfinderStepDefinitions {
    private lateinit var wayfinderReferralsAndAppointmentsPage: WayfinderReferralsAndAppointmentsPage
    private lateinit var wayfinderAggregatorErrorPage: WayfinderAggregatorErrorPage
    private lateinit var referralsOrAppointmentsHelpPage: ReferralsOrAppointmentsHelpPage
    private lateinit var confirmedAppointmentsHelpPage: ConfirmedAppointmentsHelpPage
    private lateinit var referralsInReviewHelpPage: ReferralsInReviewHelpPage

    @Steps
    private lateinit var browser: BrowserSteps

    private val wayfinderFactory = WayfinderFactory()

    @Given("^I am a user who can view Wayfinder from Appointments and has referrals but no upcoming appointments$")
    fun iAmAUserWhoHasReferralsButNoUpcomingAppointments(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsNoAppointmentsResponse()
    }

    @Given("^I am a user who can view Wayfinder from Appointments$")
    fun iAmAUserWhoCanViewWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupNoReferralsOrAppointmentsResponse();
    }

    @Given("^I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments$")
    fun iAmAUserWhoCanViewReferralsAndUpcomingAppointmentsInWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsAndUpcomingAppointmentsResponse()
    }

    @Given("^I am a user who can view Wayfinder from Appointments and receives a partial error$")
    fun iAmAUserWhoCanViewReferralsAndUpcomingAppointmentsPartialError(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsAppointmentsPartialErrorResponse()
    }

    @Given("^I am a user who is too young to use Wayfinder to retrieve referrals and appointments$")
    fun iAmAUserWhoIsTooYoungToUseWayfinderToRetrieveReferralsAndAppointments() {
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsAppointmentsUnderMinimumAgeResponse()
    }

    @Given("^I am a user who can view Wayfinder from Appointments and has eRS referrals and upcoming appointments$")
    fun iAmAUserWhoCanViewErsReferralsAndUpcomingAppointmentsInWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsErs()
    }

    @Given("^I am an eRS user who can view Wayfinder from Appointments and has PKB referrals and upcoming appointments")
    fun iAmAUserWhoCanViewPkbReferralsAndUpcomingAppointmentsInWayfinderFromTheAppointmentsHub(){
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
        wayfinderFactory.setupReferralsPkb()
    }

    @Given("^I see the Missing or incorrect referrals or appointments link")
    fun assertMissingOrIncorrectReferralsOrAppointmentsLinkIsDisplayed(){
        wayfinderReferralsAndAppointmentsPage.assertMissingOrIncorrectReferralsOrAppointmentsHelpLinkIsDisplayed()
    }

    @Given("^I see the Missing or incorrect confirmed appointments link")
    fun assertConfirmedAppointmentsLinkIsDisplayed(){
        wayfinderReferralsAndAppointmentsPage.assertConfirmedAppointmentsHelpLinkIsDisplayed()
    }

    @Given("^I see the Missing or incorrect referrals in review link")
    fun assertReferralsInReviewLinkIsDisplayed(){
        wayfinderReferralsAndAppointmentsPage.assertReferralsInReviewHelpLinkIsDisplayed()
    }

    @When("^the Wayfinder Aggregator API is timing out$")
    fun theAggregatorApiTimesOut(){
        wayfinderFactory.setupDelayedResponse()
    }

    @When("^the Wayfinder Aggregator API is encountering an issue$")
    fun theAggregatorApiEncountersAnError(){
        wayfinderFactory.setupInternalServerError()
    }

    @When("^the Wayfinder Aggregator API issues are resolved and is returning referrals and upcoming appointments$")
    fun theAggregatorApiIssuesAreResolvedAndIsReturningReferralsAndUpcomingAppointments() {
        wayfinderFactory.setupReferralsAndUpcomingAppointmentsResponse()
    }

    @When("^I click the try again button on the unavailable secondary care services error screen$")
    fun iClickTheTryAgainButtonOnTheUnavailableSecondaryCareServicesErrorScreen() {
        wayfinderAggregatorErrorPage.clickTryAgain()
    }

    @When("^I click the contact us link with the (.*) error code$")
    fun iClickTheContactUsLinkWithThePrefixErrorCode(prefix: String) {
        browser.storeCurrentTabCount()
        wayfinderAggregatorErrorPage.clickContactUsLinkWithPrefix(prefix)
    }

    @When("^I click the Missing or incorrect referrals or appointments link$")
    fun iClickTheMissingOrIncorrectReferralsOrApppointmentslink() {
        wayfinderReferralsAndAppointmentsPage.missingOrIncorrectReferralsOrAppointmentsLink.click()
    }

    @When("^I click the Missing or incorrect confirmed appointments link")
    fun iClickTheMissingOrIncorrectConfirmedApppointmentslink() {
        wayfinderReferralsAndAppointmentsPage.confirmedAppointmentsLink.click()
    }

    @When("^I click the Missing or incorrect referrals in review link")
    fun iClickTheMissingOrIncorrectReferralsInReviewlink() {
        wayfinderReferralsAndAppointmentsPage.referralsInReviewLink.click()
    }

    @Then("^the Referrals, hospital and other appointments screen with data is displayed$")
    fun assertReferralsAndAppointmentsWithDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderWithDataTitleIsDisplayed()
    }

    @Then("^the Referrals, hospital and other appointments screen without data is displayed$")
    fun assertReferralsAndAppointmentsWithoutDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderWithoutDataTitleIsDisplayed()
    }

    @Then("^I see an in-review referral$")
    fun assertInReviewReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertInReviewReferralDisplayed()
    }

    @Then("^I can see that I have no confirmed appointments$")
    fun assertNoUpcomingAppointmentsIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertNoUpcomingAppointmentsDisplayed()
    }

    @Then("^I see an appointment to confirm$")
    fun assertAppointmentToConfirmIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertApointmentToConfirmIsDisplayed()
    }

    @Then("^I see a booked appointment$")
    fun assertBookedAppointmentIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookedAppointmentIsDisplayed()
    }

    @Then("^I see a cancelled appointment$")
    fun assertCancelledAppointmentIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertCancelledAppointmentIsDisplayed()
    }

    @Then("^I see a bookable cancelled referral$")
    fun assertBookableCancelledReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookableCancelledReferralDisplayed()
    }

    @Then("^I see an overdue referral$")
    fun assertBookableOverdueReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookableOverdueReferralDisplayed()
    }

    @Then("^I see a referral awaiting booking$")
    fun assertBookableAwaitingBookIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookableAwaitingBookDisplayed()
    }

    @Then("^I see a helpful message indicating unavailable secondary care services with a (.*) service desk reference$")
    fun iSeeAHelpfulMessageIndicatingUnavailableSecondaryCareServicesWithAPrefixedServiceDeskReference(prefix: String) {
        wayfinderAggregatorErrorPage.assertIsDisplayedWithPrefix(prefix)
    }

    @Then("^I see a message indicating secondary care services are unavailable because the user is under minimum age$")
    fun iSeeAHelpfulMessageIndicatingPatientSecondaryCareServicesAreUnavailableBecauseTheUserIsUnderAge() {
        wayfinderAggregatorErrorPage.assertIsDisplayedWithUnderMinimumAgeError()
    }

    @Then("^I can see the (\\w+) referral with no speciality referenced$")
    fun iSeeTheReferralWithNoSpecialityReferenced(status: String) {
        when (status) {
            "InReview" -> wayfinderReferralsAndAppointmentsPage.assertNoSpecialityReferencedForInReview()
            "ReadyToRebook" -> wayfinderReferralsAndAppointmentsPage.assertNoSpecialityReferencedForRebook()
        }
    }

    @Then("^I am navigated to the help page for Missing or incorrect referrals or appointments$")
    fun assertReferralsOrAppointmentsHelpPageDisplayed() {
        referralsOrAppointmentsHelpPage.assertHelpPageIsDisplayed()
    }

    @Then("^I am navigated to the help page for Missing or incorrect confirmed appointments$")
    fun assertConfirmedAppointmentsHelpPageDisplayed() {
        confirmedAppointmentsHelpPage.assertHelpPageIsDisplayed()
    }

    @Then("^I am navigated to the help page for Missing or incorrect referrals in review$")
    fun assertReferralsInReviewHelpPageDisplayed() {
        referralsInReviewHelpPage.assertHelpPageIsDisplayed()
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
