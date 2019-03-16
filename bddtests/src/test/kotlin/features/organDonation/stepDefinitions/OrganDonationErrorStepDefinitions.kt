package features.organDonation.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.set
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import org.apache.http.HttpStatus
import pages.organDonation.OrganDonationErrorPage

private const val ERROR_SCENARIO = "error scenario"
private const val ERROR_SCENARIO_WILL_SUCCEED = "to succeed"
private const val OD_ERROR_RETRY_BUTTON = "Try again"

class OrganDonationErrorStepDefinitions {

    lateinit var organDonationErrorPage: OrganDonationErrorPage

    @Given("^I am a (\\w+) user registered with OD, but the ReferenceData call returns non-recoverable (.*) error$")
    fun iAmRegisteredWithOrganDonationButReferenceDataThrowsError(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        CitizenIdSessionCreateJourney(factory.mockingClient).createFor(factory.patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, factory.mockingClient).createFor(factory.patient)

        factory.mockingClient.forOrganDonation {
            referenceData().respondWithError(errorCode)
        }
    }

    @Given("^I am a (\\w+) user registered with OD, but the ReferenceData call returns recoverable (.*) error$")
    fun iAmRegisteredWithOrganDonationButReferenceDataThrowsErrorAndIRetry(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        CitizenIdSessionCreateJourney(factory.mockingClient).createFor(factory.patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, factory.mockingClient).createFor(factory.patient)
        val existingRegistration = factory.existing.optIn()
        factory.mockingClient.forOrganDonation {
            referenceData().respondWithError(errorCode)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)}

        factory.mockingClient.forOrganDonation {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(ERROR_SCENARIO_WILL_SUCCEED)}

        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithSuccess(existingRegistration)}
    }

    @Given("^I am a (\\w+) user registered with OD, but on lookup OD returns non-recoverable (.*) error$")
    fun iAmRegisteredWithODButOnLookupOrganDonationThrowsError(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(errorCode)}
    }

    @Given("^I am a (\\w+) user registered as opt-in, but on lookup OD returns recoverable (.*) error$")
    fun iAmRegisteredAsOptInButOnLookupOrganDonationThrowsErrorAndIRetry(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()

        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(errorCode)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)}

        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithSuccess(existingRegistration)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(ERROR_SCENARIO_WILL_SUCCEED)}
    }

    @Given("^I am a (\\w+) user registered with organ donation with a decision to (.*) " +
            "who wishes to withdraw but OD returns recoverable (.*) error$")
    fun iAmRegisteredWithOrganDonationAndWishToWithdrawButSeesAnError(gpSystem: String,
                                                                      decision: String,
                                                                      httpStatus: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existing = factory.existing.setUpExistingDecisionForPatient(decision)
        factory.withdrawRegistration{
            request ->request
                .respondWithError(httpStatus)
                .inScenario(ERROR_SCENARIO)
                .whenScenarioStateIs(Scenario.STARTED)
                .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)
        }
        factory.withdrawRegistration{
            request ->request
                .respondWithSuccess(existing.id)
                .inScenario(ERROR_SCENARIO)
                .whenScenarioStateIs(ERROR_SCENARIO_WILL_SUCCEED)
        }
    }

    @Given("^I am a (\\w+) user registered with organ donation with a decision to (.*) " +
            "who wishes to withdraw but OD returns non-recoverable (.*) error$")
    fun iAmRegisteredWithOrganDonationAndWishToWithdrawButSeesANonRecoverableError(gpSystem: String,
                                                                                   decision: String,
                                                                                   httpStatus: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.existing.setUpExistingDecisionForPatient(decision)
        factory.withdrawRegistration{
            request ->request.respondWithError(httpStatus)
        }
    }

    @Given("^I am a (\\w+) user who wishes to register as opt out, but OD returns non-recoverable (.*) error$")
    fun iAmAUserWhoWishesToRegisterAsOptOutButOrganDonationThrowsError(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(HttpStatus.SC_NOT_FOUND) }

        factory.create { registration -> registration.optOut {
            request ->
            request.respondWithError("test", errorCode)} }
    }

    @Given("^I am a (\\w+) user who wishes to register as opt out, but OD returns recoverable (.*) error$")
    fun iAmAUserWhoWishesToRegisterAsOptOutButOrganDonationThrowsErrorAndIRetry(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(HttpStatus.SC_NOT_FOUND) }

        factory.create { registration -> registration.optOut {
            request ->
            request.respondWithError("test", errorCode)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)} }

        factory.create { registration -> registration.optOut {
            request ->
            request.respondWithSuccess("test")
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(ERROR_SCENARIO_WILL_SUCCEED)} }
    }

    @Given("^I am a (\\w+) user registered as opt-in amends to opt-out, but OD returns non-recoverable (.*) error$")
    fun iAmRegisteredAsOptInAmendingToOptOutButOrganDonationThrowsError(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithError(existingRegistration.id,
                        errorCode)} }
    }

    @Given("^I am a (\\w+) user registered as opt-in amends to opt-out, but OD returns recoverable (.*) error$")
    fun iAmRegisteredAsOptInAmendingToOptOutButOrganDonationThrowsErrorAndIRetry(gpSystem: String, errorCode: Int) {
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        val existingRegistration = factory.existing.optIn()
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set(existingRegistration.id)

        factory.amend { registration ->
            registration.optOut { request ->
                request.respondWithError(existingRegistration.id,
                        errorCode)
                        .inScenario(ERROR_SCENARIO)
                        .whenScenarioStateIs(Scenario.STARTED)
                        .willSetStateTo(ERROR_SCENARIO_WILL_SUCCEED)} }

        factory.amend { registration-> registration.optOut {
            request ->
            request.respondWithSuccess(existingRegistration.id)
                    .inScenario(ERROR_SCENARIO)
                    .whenScenarioStateIs(ERROR_SCENARIO_WILL_SUCCEED)}}
    }

    @Given("I am a (\\w+) user registered with organ donation but existing registration is in conflicted state")
    fun iAmAUserRegisteredWithOrganDonationButExistingRegistrationIsInConflictedState(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        factory.setupPatientForAppUse()
        factory.lookUpRegistrationWithSuccessfulDemographics { a ->
            a.respondWithError(HttpStatus.SC_CONFLICT) }
    }

    @Then("^I see an appropriate Organ Donation error message without a retry option$")
    fun iSeeAnAppropriateOrganDonationErrorMessageWithNoOptionToRetry() {
        organDonationErrorPage.assertHeaderText(organDonationErrorPage.errorHeader)
                .assertMessageText(organDonationErrorPage.errorMessageNoRetry)
                .assertNoButton(OD_ERROR_RETRY_BUTTON)
    }

    @Then("^I see an appropriate Organ Donation error message with a retry option$")
    fun iSeeAnAppropriateOrganDonationErrorMessageWithOptionToRetry() {
        organDonationErrorPage.assertHeaderText(organDonationErrorPage.errorHeader)
                .assertMessageText(organDonationErrorPage.errorMessageWithRetry)
                .assertHasButton(OD_ERROR_RETRY_BUTTON)
    }
}
