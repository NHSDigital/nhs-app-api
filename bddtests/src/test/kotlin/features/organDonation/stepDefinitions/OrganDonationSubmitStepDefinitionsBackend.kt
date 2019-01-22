package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.organDonation.models.OrganDonationRegistrationRequest
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.organdonation.OrganDonationRegistrationResponse

class OrganDonationSubmitStepDefinitionsBackend {

    @Given("^I am a (.*) user who wants to opt-out of organ donation$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val registrationId = "NewOrganDonationId"
        Serenity.setSessionVariable("ExpectedOrganDonationRegistrationId").to(registrationId)
        factory.optOut { registration -> registration.respondWithSuccess(registrationId) }
    }

    @Given("^I am a (.*) user who wants to opt-in to organ donation$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptIn(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val registrationId = "NewOrganDonationId"
        Serenity.setSessionVariable("ExpectedOrganDonationRegistrationId").to(registrationId)
        factory.optIn { registration -> registration.respondWithSuccess(registrationId) }
    }

    @Given("^I am a (.*) user who wants to donate some but not all organs$")
    fun iAmAUserWhoWantsToDonateSomeButNotAllOrgans(gpSystem: String){
        val factory = OrganDonationFactory(gpSystem)
        val registrationId = "NewOrganDonationId"
        Serenity.setSessionVariable("ExpectedOrganDonationRegistrationId").to(registrationId)
        factory.some { registration -> registration.respondWithSuccess(registrationId) }
    }

    @Given("^I am a (.*) user who wants to opt-out of organ donation, but OD will time out$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptOutButOrganDonationWillTimeOut(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.optOut { registration -> registration.respondWithTimeOutError() }
    }

    @Given("^I am a (.*) user who wants to opt-out of organ donation, but OD will return an internal error$")
    fun iAmNotRegisteredWithOrganDonationWhoChoosesToOptOutButOrganDonationWillReturnInternalError(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.optOut { registration -> registration.respondWithInternalError() }
    }

    @When("^I submit my decision to organ donation$")
    fun iSubmitMyDecisionToOrganDonation() {
        val registration = Serenity.sessionVariableCalled<OrganDonationRegistrationRequest>(
                ORGAN_DONATION_DECISION)
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .postRegistration(registration)
            setSessionVariable(OrganDonationRegistrationResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I submit a request to set my organ donation preferences with all organs and my faiths and beliefs decision")
    fun iSubmitARequestToSetMyOrganDonationPreferencesWithAllOrgansAndMyFaithsAndBeliefsDecision() {
        val registration = Serenity.sessionVariableCalled<OrganDonationRegistrationRequest>(
                ORGAN_DONATION_DECISION)
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

    @Then("^I receive my registration id from organ donation$")
    fun iReceiveMyRegistrationIdFromOrganDonation() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationRegistrationResponse>(OrganDonationRegistrationResponse::class)
        val expected = Serenity.sessionVariableCalled<String>("ExpectedOrganDonationRegistrationId")
        Assert.assertEquals("Expected Organ Donation Registration Id",
                expected,
                organDonationResponse.identifier)
    }
}
