package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.organDonation.OrganDonationWithdrawalResponse
import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.OrganDonationWithdrawRequest
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.organdonation.OrganDonationRegistrationResponse

class OrganDonationSubmitStepDefinitionsBackend {

    @Given("^I am a (\\w+) api user who wants to opt-out of organ donation$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set("NewOrganDonationId")
        factory.create { registration->registration.optOut {
            request -> request.respondWithSuccess("NewOrganDonationId") }}
    }

    @Given("^I am a (\\w+) api user who wants to opt-in to organ donation$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set("NewOrganDonationId")
        factory.create { registration->registration.optIn {
            request -> request.respondWithSuccess("NewOrganDonationId") }}
    }

    @Given("^I am a (\\w+) api user who wants to donate some but not all organs$")
    fun iAmAUserWhoWantsToDonateSomeButNotAllOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.set("NewOrganDonationId")
        factory.create { registration ->
            registration.some(OrganDonationRegistrationDataBuilder.someOrgansListUpdated())
            { request -> request.respondWithSuccess("NewOrganDonationId") }
        }
    }

    @When("^I submit my decision to organ donation$")
    fun iSubmitMyDecisionToOrganDonation() {
        val registration = OrganDonationSerenityHelpers.ORGAN_DONATION_DECISION
                .getOrFail<OrganDonationRegistrationRequest>()
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .postRegistration(registration)
            setSessionVariable(OrganDonationRegistrationResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I submit my updated decision to organ donation$")
    fun iSubmitMyUpdatedDecisionToOrganDonation() {
        val registration = OrganDonationSerenityHelpers.ORGAN_DONATION_DECISION
                .getOrFail<OrganDonationRegistrationRequest>()
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .putRegistration(registration)
            setSessionVariable(OrganDonationRegistrationResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I submit a request to set my organ donation preferences with all organs and my faiths and beliefs decision")
    fun iSubmitARequestToSetMyOrganDonationPreferencesWithAllOrgansAndMyFaithsAndBeliefsDecision() {
        val registration = OrganDonationSerenityHelpers.ORGAN_DONATION_DECISION
                .getOrFail<OrganDonationRegistrationRequest>()
        registration.registration.faithDeclaration = "Yes"
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .postRegistration(registration)
            setSessionVariable(OrganDonationRegistrationResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I submit my organ donation withdraw decision$")
    fun iSubmitMyOrganDonationWithdrawDecision() {
        val withdrawalRequestBody = OrganDonationSerenityHelpers.ORGAN_DONATION_WITHDRAWAL
                .getOrFail<OrganDonationWithdrawRequest>()
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .deleteRegistration(withdrawalRequestBody)
            setSessionVariable(OrganDonationWithdrawalResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive my registration id from organ donation$")
    fun iReceiveMyRegistrationIdFromOrganDonation() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationRegistrationResponse>(OrganDonationRegistrationResponse::class)
        val expected = OrganDonationSerenityHelpers.EXPECTED_REGISTRATION_ID.getOrFail<String>()
        Assert.assertEquals("Expected Organ Donation Registration Id",
                expected,
                organDonationResponse.identifier)
    }
}
