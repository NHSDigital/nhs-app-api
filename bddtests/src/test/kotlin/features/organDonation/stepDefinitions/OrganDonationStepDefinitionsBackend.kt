package features.organDonation.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.organdonation.OrganDonationSearchResponse
import worker.models.organdonation.OrganDonationState

class OrganDonationStepDefinitionsBackend {

    @Given("I am a (\\w+) api user registered with organ donation to not donate my organs")
    fun iAmRegisteredWithOrganDonationToNotDonateOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.existing.optOut()
    }

    @Given("I am a (\\w+) api user registered with organ donation to donate all organs")
    fun iAmRegisteredWithOrganDonationToDonateAllOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.existing.optIn()
    }

    @Given("I am a (\\w+) api user registered with organ donation with an appointed representative")
    fun iAmRegisteredWithOrganDonationWithAnAppointedRepresentative(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.existing.appointedRepresentative()
    }

    @Given("I am a (\\w+) api user registered with organ donation to donate some organs")
    fun iAmRegisteredWithOrganDonationToDonateSomeOrgans(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.existing.optInSome()
    }

    @Given("^I am a (\\w+) api user registered with an organ " +
            "donation decision to (.*) and wish to withdraw my decision$")
    fun iAmRegisteredWithOrganDonationAndWishToWithdraw(gpSystem: String, decision:String){
        val supplier = Supplier.valueOf(gpSystem)
        val factory = OrganDonationFactory(supplier)
        factory.setupPatientForAppUse()
        val existing = factory.existing.setUpExistingDecisionForPatient(decision)
        factory.withdrawRegistration{
            request ->request.respondWithSuccess(existing.id)
        }
    }

    @Given("^I am a (\\w+) api user not registered with organ donation$")
    fun iAmNotRegisteredWithOrganDonation(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        OrganDonationFactory(supplier).lookUpRegistrationWithSuccessfulDemographics { a->
            a.respondWithError(HttpStatus.SC_NOT_FOUND)}
    }

    @When("^I request my organ donation details$")
    fun iRequestMyOrganDonationDetails() {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .getOrganDonationConnection(patientId)
            setSessionVariable(OrganDonationSearchResponse::class).to(response)
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
}
