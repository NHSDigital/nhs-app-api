package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.AllergiesData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.models.myrecord.MyRecordResponse

open class MyRecordAllergiesStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance

    @Given("the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergies(count: Int) {
        mockingClient.forEmis {
            allergiesRequest(MockDefaults.patient).respondWithSuccess(AllergiesData.getAllergiesData(count))
        }
    }

    @But("the GP Practice has disabled allergies functionality")
    fun butTheGPPracticeHasDisabledAllergiesFunctionality() {
        mockingClient.forEmis {
            allergiesRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
        }
    }

    @Then("I receive \"(.*)\" allergies as part of the my record object")
    fun thenIReceiveAnAllergiesObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.allergies?.data?.count())
    }

    @And("the flag informing that the patient has access to the allergy data is set to \"(.*)\"")
    fun andHasAccessToAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies?.hasAccess)
    }

    @And("the flag informing that there was an error retrieving the allergy data is set to \"(.*)\"")
    fun andHasErrorsWhenRetrievingAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies?.hasErrored)
    }
}

