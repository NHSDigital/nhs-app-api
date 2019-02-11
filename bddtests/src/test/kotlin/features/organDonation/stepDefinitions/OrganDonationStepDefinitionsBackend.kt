package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.stubs.StubbedEnvironment
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.organdonation.OrganDonationSearchResponse
import worker.models.organdonation.OrganDonationState
import java.time.Duration

class OrganDonationStepDefinitionsBackend {

    @Given("I am a (\\w+) api user registered with organ donation to not donate my organs")
    fun iAmRegisteredWithOrganDonationToNotDonateOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.existingOptOut()
    }

    @Given("I am a (\\w+) api user registered with organ donation to donate all organs")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.existingOptIn()
    }

    @Given("I am a (\\w+) api user registered with organ donation with an appointed representative")
    fun iAmRegisteredWithOrganDonationWithAnAppointedRepresentative(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.existingAppointedRepresentative()
    }

    @Given("I am a (\\w+) api user registered with organ donation to donate some organs")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgans(gpSystem: String) {
        val factory = OrganDonationFactory(gpSystem)
        factory.existingOptInSome()
    }

    @Given("^I am a (\\w+) api user not registered with organ donation$")
    fun iAmNotRegisteredWithOrganDonation(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics { a->a.respondWithNotFoundError()}
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but organ donation will conflict$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillConflict(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithConflictError()}
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but lookup call will time out$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillThrowTimeout(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithTimeOutError()
                .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))}
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but lookup call will return an internal error$")
    fun iAmRegisteredWithOrganDonationButOrganDonationWillThrowInternalError(gpSystem: String) {
        OrganDonationFactory(gpSystem).lookUpRegistrationWithSuccessfulDemographics{ a->a.respondWithInternalError()}
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but demographics will time out$")
    fun iAmRegisteredWithOrganDonationButDemographicsWillThrowTimeOutError(gpSystem: String) {
        OrganDonationFactory(gpSystem).demographicsTimeout()
    }

    @Given("^I am a (\\w+) api user registered with organ donation, but demographics will return an internal error$")
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

    @Then("^I receive organ donation details with an '(.*)' decision$")
    fun iReceiveOrganDonationDetails(expectedDecision : String) {
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)

        Assert.assertNotEquals("Organ donation decision incorrect",
                "Unknown",
                organDonationResponse.decision)
        Assert.assertEquals("Organ donation decision incorrect",
                expectedDecision,
                organDonationResponse.decision)
        Assert.assertEquals("Organ donation decision incorrect",
                expectedDecision,
                organDonationResponse.decision)
        Assert.assertNotNull("Organ donation identifier was not found", organDonationResponse.identifier)
        Assert.assertEquals("State",
                OrganDonationState.Ok,
                organDonationResponse.state)
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
        Assert.assertEquals("State",
                OrganDonationState.NotFound,
                organDonationResponse.state)
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

    @Then("^I receive an organ donation response with state value of 'conflicted'")
    fun iReceiveAnOrganDonationResponseWithStateValueOfConflicted(){
        val organDonationResponse = Serenity
                .sessionVariableCalled<OrganDonationSearchResponse>(OrganDonationSearchResponse::class)
        Assert.assertEquals("State",
                OrganDonationState.Conflicted,
                organDonationResponse.state)

    }
}
