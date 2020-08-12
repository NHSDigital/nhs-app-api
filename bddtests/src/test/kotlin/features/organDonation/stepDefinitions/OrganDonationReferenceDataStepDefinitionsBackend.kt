package features.organDonation.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.data.organDonation.OrganDonationReferenceDataBuilder
import mocking.organDonation.models.ReferenceDataResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.WorkerClient

class OrganDonationReferenceDataStepDefinitionsBackend {

    @Given("^I am a (\\w+) api user not registered with organ donation, where the reference data call will " +
            "return data$")
    fun iAmRegisteredWithOrganDonationAndReferenceDataWillBeSuccessful(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        OrganDonationFactory(supplier).mockingClient.forOrganDonation.mock {
            referenceData().respondWithSuccess(OrganDonationReferenceDataBuilder.build())
        }
    }

    @When("^I request the Organ Donation Reference Data$")
    fun iRequestOrganDonationReferenceData() {
        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .organDonation
                .getOrganDonationReferenceData()
        Serenity.setSessionVariable(ReferenceDataResponse::class).to(response)
    }

    @Then("^I receive Organ Donation Reference Data$")
    fun iReceiveOrganDonationReferenceData() {
        val organDonationResponse =
                Serenity.sessionVariableCalled<ReferenceDataResponse>(ReferenceDataResponse::class)
        Assert.assertNotNull(organDonationResponse)
    }
}
