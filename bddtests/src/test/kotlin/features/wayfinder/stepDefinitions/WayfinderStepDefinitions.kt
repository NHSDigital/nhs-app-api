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
import pages.wayfinder.help.WayfinderHelpPage
import pages.wayfinder.waitTimes.WayfinderWaitTimesPage
import utils.SerenityHelpers

class WayfinderStepDefinitions {
    private lateinit var wayfinderReferralsAndAppointmentsPage: WayfinderReferralsAndAppointmentsPage
    private lateinit var wayfinderAggregatorErrorPage: WayfinderAggregatorErrorPage
    private lateinit var wayfinderHelpPage: WayfinderHelpPage
    private lateinit var wayfinderWaitTimesPage: WayfinderWaitTimesPage

    @Steps
    private lateinit var browser: BrowserSteps

    private val wayfinderFactory = WayfinderFactory()

    @Given("^I am a user whose surgery has enabled Wayfinder$")
    fun iAmAUserWhoseSurgeryHadEnabledWayfinder() {
        setupPatient(SJRJourneyType.WAYFINDER_ENABLED)
    }

    @Given("^I have no referrals or appointments$")
    fun iHaveNoReferralsOrAppointments(){
        wayfinderFactory.setupNoReferralsOrAppointmentsResponse()
    }

    @Given("^I have referrals but no upcoming appointments$")
    fun iHaveReferralsButNoUpcomingAppointments(){
        wayfinderFactory.setupReferralsNoAppointmentsResponse()
    }

    @Given("^I have referrals and upcoming appointments$")
    fun iHaveReferralsAndUpcomingAppointments(){
        wayfinderFactory.setupReferralsAndUpcomingAppointmentsResponse()
    }

    @Given("^I get a partial error returned for a supplier$")
    fun iGetAPartialErrorReturnedForASupplier(){
        wayfinderFactory.setupReferralsAppointmentsPartialErrorResponse()
    }

    @Given("^I am too young to use Wayfinder to retrieve referrals and appointments$")
    fun iAmTooYoungToUseWayfinderToRetrieveReferralsAndAppointments() {
        wayfinderFactory.setupReferralsAppointmentsUnderMinimumAgeResponse()
    }

    @Given("^I have referrals and upcoming (AccurxWayfinder|DrDoctor|HealthcareComms|Netcall|PKB|Zesty) appointments$")
    fun iHaveReferralsAndUpcomingAppointments(provider: String){
        wayfinderFactory.setupReferralsAndAppointments(provider)
    }

    @Given("^I see the Missing, incorrect or not cancelled referrals or appointments link$")
    fun iSeeTheMissingOrIncorrectReferralsOrAppointmentsLink(){
        wayfinderReferralsAndAppointmentsPage.assertReferralsOrAppointmentsHelpLinkIsDisplayed()
    }

    @Given("^I see the Missing, incorrect or not cancelled appointments link$")
    fun iSeeTheMissingOrIncorrectAppointmentsLink(){
        wayfinderReferralsAndAppointmentsPage.assertAppointmentsHelpLinkIsDisplayed()
    }

    @Given("^I see the Missing or incorrect referrals in review link$")
    fun iSeeTheMissingOrIncorrectReferralsInReviewLink(){
        wayfinderReferralsAndAppointmentsPage.assertReferralsInReviewHelpLinkIsDisplayed()
    }

    @Given("^I have wait times available$")
    fun iHaveWaitTimesAvailable(){
        wayfinderFactory.setupWaitTimes()
    }

    @Given("^I see the wait times link$")
    fun iSeeTheWaitTimesLink(){
        wayfinderReferralsAndAppointmentsPage.assertWaitTimesLinkIsDisplayed()
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

    @When("^I click the Missing, incorrect or not cancelled referrals or appointments link$")
    fun iClickTheMissingOrIncorrectReferralsOrApppointmentsLink() {
        wayfinderReferralsAndAppointmentsPage.missingOrIncorrectReferralsOrAppointmentsLink.click()
    }

    @When("^I click the Missing, incorrect or not cancelled appointments link$")
    fun iClickTheMissingOrIncorrectAppointmentsLink() {
        wayfinderReferralsAndAppointmentsPage.appointmentsLink.click()
    }

    @When("^I click the Missing or incorrect referrals in review link$")
    fun iClickTheMissingOrIncorrectReferralsInReviewLink() {
        wayfinderReferralsAndAppointmentsPage.referralsInReviewLink.click()
    }

    @When("^I click the '(.*)' expander link on a Wayfinder Help page$")
    fun iClickTheExpanderLinkOnThePage(expanderText: String) {
        wayfinderHelpPage.clickExpanderLink(expanderText)
    }

    @When("^I click the wait times jump off link$")
    fun iClickTheWaitTimesJumpOffLink() {
        wayfinderReferralsAndAppointmentsPage.waitTimesLink.click()
    }

    @Then("^the Referrals, hospital and other appointments screen with data is displayed$")
    fun assertReferralsAndAppointmentsWithDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderTitleIsDisplayed()
    }

    @Then("^the Referrals, hospital and other appointments screen without data is displayed$")
    fun assertReferralsAndAppointmentsWithoutDataPageIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertWayfinderTitleIsDisplayed()
    }

    @Then("^I see an in-review referral$")
    fun assertInReviewReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertReferralInReviewIsDisplayed()
    }

    @Then("^I can see that I have no confirmed appointments$")
    fun assertNoUpcomingAppointmentsIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertNoConfirmedAppointmentsMessageIsDisplayed()
    }

    @Then("^I see an appointment ready to confirm$")
    fun assertAppointmentReadyToConfirmIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertAppointmentReadyToConfirmIsDisplayed()
    }

    @Then("^I see a booked appointment$")
    fun assertBookedAppointmentIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertBookedAppointmentIsDisplayed()
    }

    @Then("^I see a cancelled appointment$")
    fun assertCancelledAppointmentIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertAppointmentCancelledIsDisplayed()
    }

    @Then("^I see a bookable cancelled referral$")
    fun assertBookableCancelledReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertReferralReadyToRebookIsDisplayed()
    }

    @Then("^I see an overdue referral$")
    fun assertBookableOverdueReferralIsDisplayed() {
        wayfinderReferralsAndAppointmentsPage.assertReferralInReviewOverdueIsDisplayed()
    }

    @Then("^I see a referral that is ready to book$")
    fun iSeeAReferralThatIsReadyToBook() {
        wayfinderReferralsAndAppointmentsPage.assertReferralReadyToBookIsDisplayed()
    }

    @Then("^I see a helpful message indicating unavailable secondary care services with a (.*) service desk reference$")
    fun iSeeAHelpfulMessageIndicatingUnavailableSecondaryCareServicesWithAPrefixedServiceDeskReference(prefix: String) {
        wayfinderAggregatorErrorPage.assertIsDisplayedWithPrefix(prefix)
    }

    @Then("^I see a message indicating secondary care services are unavailable because the user is under minimum age$")
    fun iSeeAHelpfulMessageIndicatingPatientSecondaryCareServicesAreUnavailableBecauseTheUserIsUnderAge() {
        wayfinderAggregatorErrorPage.assertIsDisplayedWithUnderMinimumAgeError()
    }

    @Then("^I can see the (\\w+) referral with no specialty referenced$")
    fun iSeeTheReferralWithNoSpecialtyReferenced(status: String) {
        when (status) {
            "InReview" -> wayfinderReferralsAndAppointmentsPage
                .assertReferralInReviewWithNoSpecialtyIsDisplayed()
            "ReadyToRebook" -> wayfinderReferralsAndAppointmentsPage
                .assertReferralReadyToRebookWithNoSpecialtyIsDisplayed()
        }
    }

    @Then("^I am navigated to the Wayfinder help page$")
    fun assertWayfinderHelpPageDisplayed() {
        wayfinderHelpPage.assertHelpPageIsDisplayed()
    }

    @Then("^I am navigated to the Wayfinder wait times page$")
    fun assertWayfinderWaitTimesPageDisplayed() {
        wayfinderWaitTimesPage.assertWayfinderWaitTimesTitleIsDisplayed()
    }

    @Then("^I can see the missing referrals expander elements displayed$")
    fun assertWayfinderHelpMissingReferralsExpanderElementsDisplayed() {
        wayfinderHelpPage.assertMissingReferralsExpanderElementsDisplayed()
    }

    @Then("^I can see the incorrect or cancelled referrals expander elements displayed$")
    fun assertWayfinderHelpIncorrectOrCancelledReferralsExpanderElementsDisplayed() {
        wayfinderHelpPage.assertIncorrectOrCancelledReferralsExpanderElementsDisplayed()
    }

    @Then("^I can see the missing appointments expander expander elements displayed$")
    fun assertWayfinderHelpMissingAppointmentsExpanderExpanderElementsDisplayed() {
        wayfinderHelpPage.assertMissingAppointmentsExpanderExpanderElementsDisplayed()
    }

    @Then("^I can see the incorrect changed cancelled appointments expander elements displayed$")
    fun assertWayfinderHelpIncorrectChangedCancelledAppointmentsExpanderElementsDisplayed() {
        wayfinderHelpPage.assertIncorrectChangedCancelledAppointmentsExpanderElementsDisplayed()
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
