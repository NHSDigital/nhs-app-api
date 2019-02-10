package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationRegistrationDataBuilder
import mocking.stubs.StubbedEnvironment
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.organdonation.OrganDonationSearchResponse
import java.time.Duration

class OrganDonationStepDefinitionsBackend {

    @Given("^I am a (.*) user registered with organ donation$")
    fun iAmAlreadyRegisteredWithOrganDonation(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        val patient = factory.patient
        val registration = OrganDonationRegistrationDataBuilder.optOut(patient)
        factory.lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithSuccess(registration)}
    }

    @Given("^I am a (.*) user not registered with organ donation$")
    fun iAmNotRegisteredWithOrganDonation(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics { a->a.respondWithNotFoundError()}
    }

    @Given("^I am a (.*) user registered with organ donation, but organ donation will conflict$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillConflict(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithConflictError()}
    }

    @Given("^I am a (.*) user registered with organ donation, but organ donation call will time out$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillThrowTimeout(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithTimeOutError()
                .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))}
    }

    @Given("^I am a (.*) user registered with organ donation, but organ donation call will return an internal error$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillThrowInternalError(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithInternalError()}
    }

    @Given("^I am a (.*) user registered with organ donation, but demographics will time out$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowTimeOutError(gpSystem: String) {
        OrganDonationFactory(gpSystem).demographicsTimeout()
    }

    @Given("^I am a (.*) user registered with organ donation, but demographics will return an internal error$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowInternalError(gpSystem: String) {
        OrganDonationFactory(gpSystem).demographicsInternalError()
    }

    @When("^I request my organ donation details$")
    fun iRequestMyOrganDonationDetails() {
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .getOrganDonationConnection()
            setSessionVariable(OrganDonationSearchResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive organ donation details$")
    fun iReceiveOrganDonationDetails() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertNotEquals("Organ donation decision incorrect",
                "Unknown",
                organDonationResponse.decision)
        Assert.assertNotNull("Organ donation identifier was not found", organDonationResponse.identifier)
    }

    @Then("^I receive no organ donation details$")
    fun iDoNotReceiveOrganDonationDetail() {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertEquals("Organ donation decision incorrect",
                "Unknown",
                organDonationResponse.decision)
        Assert.assertNull("Organ donation identifier should be null",
                organDonationResponse.identifier)
    }

    @Then("^I receive the users demographics details$")
    fun iReceiveTheGpUsersDemographicsDetails() {
        val patient = SerenityHelpers.getPatient()

        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertEquals("Nhs number in response does not match the patient",
                patient.formattedNHSNumber(),
                organDonationResponse.nhsNumber)
        Assert.assertEquals("Patient name in the response does not match patient",
                patient.formattedFullName(),
                organDonationResponse.nameFull)
    }
}
