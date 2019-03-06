package features.organDonation.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.organDonation.models.ReferenceDataResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient

class OrganDonationReferenceDataStepDefinitionsBackend {

    @Given("^I am a (\\w+) api user not registered with organ donation, where the reference data call will " +
            "return data$")
    fun iAmRegisteredWithOrganDonationAndReferenceDataWillBeSuccessful(gpSystem: String) {
        OrganDonationFactory(gpSystem).mockingClient.forOrganDonation {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
        }
    }

    @When("^I request the Organ Donation Reference Data$")
    fun iRequestOrganDonationReferenceData() {
        try {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .organDonation
                    .getOrganDonationReferenceData()
            Serenity.setSessionVariable(ReferenceDataResponse::class).to(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive Organ Donation Reference Data$")
    fun iReceiveOrganDonationReferenceData() {
        val organDonationResponse =
                Serenity.sessionVariableCalled<ReferenceDataResponse>(ReferenceDataResponse::class)
        Assert.assertNotNull(organDonationResponse)
    }
}
