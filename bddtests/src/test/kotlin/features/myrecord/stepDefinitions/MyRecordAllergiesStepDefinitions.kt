package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.data.myrecord.AllergiesData
import mocking.tpp.models.Error
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.models.myrecord.MyRecordResponse

open class MyRecordAllergiesStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has enabled allergies functionality and the patient has \"(.*)\" allergies for (.*)$")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndPatientHasSomeAllergiesForService(count: Int, getService: String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" ->

                mockingClient.forEmis {
                    allergiesRequest(this@MyRecordAllergiesStepDefinitions.patient).respondWithSuccess(AllergiesData.getEmisAllergiesData(count))
                }
            "TPP" ->{
                mockingClient.forTpp {
                    viewPatientOverviewPost(this@MyRecordAllergiesStepDefinitions.patient.tppUserSession!!).respondWithSuccess(AllergiesData.getTppAllergiesData(count))
                }
            }
        }
    }

    @Given("the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats for (.*)")
    fun givenTheGPPracticeHasEnabledAllergiesFunctionalityAndHasFiveDifferentAllergiesWithDifferentDateFormatsForService(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" ->
                mockingClient.forEmis {
                    allergiesRequest(this@MyRecordAllergiesStepDefinitions.patient).respondWithSuccess(AllergiesData.getEmisAllergyRecordsWithDifferentDateParts())
                }
            "TPP" -> {
                mockingClient.forTpp {
                    viewPatientOverviewPost(this@MyRecordAllergiesStepDefinitions.patient.tppUserSession!!).respondWithSuccess(AllergiesData.getTppAllergiesData(5))
                }
            }
        }
    }

    @But("the GP Practice has disabled allergies functionality for (.*)")
    fun butTheGPPracticeHasDisabledAllergiesFunctionalityForService(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    allergiesRequest(this@MyRecordAllergiesStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    viewPatientOverviewPost(this@MyRecordAllergiesStepDefinitions.patient.tppUserSession!!).respondWithError(Error("6", "Requested record access is disabled by the practice", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }

    @Then("^I receive \"(.*)\" allergies as part of the my record object$")
    fun thenIReceiveAnAllergiesObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.allergies.data.count())
    }

    @And("^the flag informing that the patient has access to the allergy data is set to \"(.*)\"$")
    fun andHasAccessToAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the allergy data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingAllergiesDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.allergies.hasErrored)
    }
}

